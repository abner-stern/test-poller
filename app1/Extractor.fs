module Extractor

open System.Text.RegularExpressions

let to_number (text: string) =
    match System.Double.TryParse text with
        | false, _ ->
            None
        | true, x ->
            Some x

let extract_temperature text =
    let m = Regex.Match(text, "((?:[+-]?)[\d.]+)")
    match m.Success with
        | true ->
            to_number m.Captures.[0].Value
        | false ->
            None

let split_by_markers text beginning_marker ending_marker =
    let parts = Regex.Split(text, beginning_marker)
    match parts.Length with
        | n when n > 1 ->
            let tail = parts.[1]
            let parts2 = Regex.Split(tail, ending_marker)
            match parts2.Length with
                | n when n > 1 ->
                    let head = parts2.[0]
                    Some head
                | _ ->
                    None
        | _ ->
            None

let extract_today_temperature text =
    let beginning = """(?is)<li\s[^<>]+\sdata-pos="0"[^<>]*>"""
    let ending = "(?i)</li>"
    let part = split_by_markers text beginning ending
    match part with
        | Some inner_text ->
            let m = Regex.Match(inner_text, """(?is)<span class="wr-value--temperature--c">(.*)</span>""")
            extract_temperature m.Captures.[0].Value
        | _ ->
            None

let to_date_time (text: string) =
    match System.DateTime.TryParse text with
        | false, _ ->
            None
        | true, x ->
            Some x

let extract_time text =
    let m = Regex.Match(text, "at\s(\d\d:\d\d)")
    match m.Success with
        | true when m.Groups.Count > 1 ->
            m.Groups.[1].Value
        | _ ->
            ""

let extract_last_update_time text =
    let beginning = """(?is)<div\s[^<>]+last-updated\s[^<>]+>"""
    let ending = "(?i)</div>"
    let part = split_by_markers text beginning ending
    match part with
        | Some inner_text ->
            let m = Regex.Match(inner_text, """(?is)<time>(.*)</time>""")
            m.Captures.[0].Value
                |> extract_time
                |> to_date_time
        | _ ->
            None

let extract text =
    let today_temperature = extract_today_temperature text
    let last_updated = extract_last_update_time text
    today_temperature, last_updated

