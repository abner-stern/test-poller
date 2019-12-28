module App

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open System.Threading
open System.Threading.Tasks

open Worker

type Capp (log: ILogger<IHostedService>,
           lifetime: IHostApplicationLifetime,
           srv: WorkService) =

    let _log = log
    let _lifetime = lifetime
    let mutable _srv = srv
    do
        _log.LogInformation "capp created by constructor"

    let on_started () =
        _log.LogInformation "capp on_started"
        ()

    let on_stopped () =
        _log.LogInformation "capp on_stopped"
        ()

    let on_stopping () =
        _log.LogInformation "capp on_stopping"
        ()

    interface IHostedService with

        override this.StartAsync (cancellationToken: CancellationToken) =
            _log.LogInformation "capp start async"
            _lifetime.ApplicationStarted.Register (System.Action on_started) |> ignore
            _lifetime.ApplicationStopping.Register (System.Action on_stopping) |> ignore
            _lifetime.ApplicationStopped.Register (System.Action on_stopped) |> ignore
            _log.LogInformation "capp start async done"
            Task.CompletedTask
        override this.StopAsync (cancellationToken: CancellationToken) =
            _log.LogInformation "capp stop async"
            Task.CompletedTask

    // member this.Run () =
    //         printfn "Running the app."
    //         _srv.do_work ()
    //         ()


