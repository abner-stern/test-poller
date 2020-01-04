module Poller

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Http
open Microsoft.Extensions.Logging
open System
open System.Net.Http
open System.Threading
open System.Threading.Tasks

open Extractor
open Item
open ItemContext
open Alog
open MeasureItem

type Poller (log: ILogger<Poller>,
             httpFactory: IHttpClientFactory,
             db: ItemContext) =
    inherit BackgroundService ()

    let _db = db
    let _httpFactory = httpFactory
    let _log = log
    let mutable _cnt = 0

    let on_stop () =
        _log.LogInformation "Poller stopping???"
        ()

    let get_one_page (httpFactory: IHttpClientFactory) =
        let url = "https://www.bbc.com/weather/2643743"
        let request = new HttpRequestMessage(HttpMethod.Get, url)
        let client = httpFactory.CreateClient ()
        _log.LogInformation (DateTime.Now.TimeOfDay.ToString() + " get_one_page")
        async {
            let! response = client.SendAsync request |> Async.AwaitTask
            return response
        }
        |> Async.Catch
        |> Async.RunSynchronously

    let read_body (resp: HttpResponseMessage) =
        async {
            let! text = resp.Content.ReadAsStringAsync () |> Async.AwaitTask<string>
            return text
        }
        |> Async.Catch
        |> Async.RunSynchronously


    let build_ok_result temperature last_updated =
        match temperature, last_updated with
            | Some t, Some upd ->
                Some {LastUpdated = upd; Temperature = t}
            | _ ->
                None

    let add_item_to_context_if_not_exists item =
        try
            let result = _db.MeasureItems.Find item.LastUpdated
            Alog.info(logger=_log, result="already exists, " + result.ToString())
        with
            | :? NullReferenceException as ex ->
                let result = _db.MeasureItems.Add item
                Alog.info(logger=_log, result="added, " + result.ToString())
            | _ as ex ->
                Alog.info(logger=_log, result="exception, " + ex.ToString())
        ()

    let store_result some_measure_item =
        match some_measure_item with
            | Some item ->
                try
                    add_item_to_context_if_not_exists item
                    let written = _db.SaveChanges() // can throw 'duplicate key' because of other thread
                    Alog.info(logger=_log, result="item added, " + written.ToString())
                with
                    | _ as ex ->
                        Alog.info(logger=_log, result="exception, " + ex.ToString())
            | None ->
                ()

    let handle_ok_result (result: HttpResponseMessage) =
        match result.StatusCode with
            | Net.HttpStatusCode.OK ->
                let response_body = read_body result
                match response_body with
                    | Choice1Of2 text ->
                        let (temperature, last_updated) = extract text
                        let measure_item = build_ok_result temperature last_updated
                        store_result measure_item
                    | Choice2Of2 exc ->
                        _log.LogInformation ("handle_ok_result, exc: " + exc.ToString())
            | other ->
                _log.LogInformation ("handle_ok_result, other status: " + other.ToString())

    let handle_exception_result exc =
        "exc: " + exc.ToString ()

    let do_one_step (httpFactory: IHttpClientFactory) (i: int) =
        _log.LogInformation (DateTime.Now.TimeOfDay.ToString()
                             + " do_one_step " + i.ToString())
        let resp = get_one_page httpFactory
        match resp with
            | Choice1Of2 result ->
                handle_ok_result result
            | Choice2Of2 exc ->
                let res = handle_exception_result exc
                _log.LogInformation ("exc result: " + res)

    let rec do_work (cancellationToken: CancellationToken) =
        _log.LogInformation (DateTime.Now.TimeOfDay.ToString()
                             + " Poller do work start, cnt: "
                             + _cnt.ToString())
        let cur_item = {Id = 0; UserId = _cnt; Name = "name " + _cnt.ToString()}
        let result = _db.Items.Add cur_item
        _db.SaveChanges() |> ignore
        let len = _db.items |> Seq.length
        match cancellationToken.IsCancellationRequested, _cnt with
            | _, 7 -> ()
            | true, _ -> ()
            | false, _ ->
                do_one_step _httpFactory _cnt |> ignore
                _cnt <- _cnt + 1
                Task.Delay(1000, cancellationToken).Wait()
                do_work cancellationToken

    let run_poller cancellationToken =
        try
            do_work cancellationToken
        with
            | _ as ex ->
                Alog.info(logger=_log, result="exception, " + ex.ToString())

    override this.ExecuteAsync (cancellationToken: CancellationToken) =
        _log.LogInformation "Poller is starting"
        cancellationToken.Register (System.Action on_stop) |> ignore
        Task.Run (System.Action (fun () -> run_poller cancellationToken))

