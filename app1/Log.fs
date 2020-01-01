module Alog

open Microsoft.Extensions.Logging
open System
open System.Diagnostics
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type Alog () =

    static member info (logger: ILogger,
                        [<CallerMemberName; Optional; DefaultParameterValue("")>] caller: string,
                        [<CallerFilePath; Optional; DefaultParameterValue("")>] path: string,
                        [<CallerLineNumberAttribute; Optional; DefaultParameterValue(0)>] line: int,
                        result) =
        logger.Log (
            LogLevel.Information,
            "{Timestamp}, {Caller}, {File}, {Line}, result: {Result}",
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
            caller,
            path,
            line,
            result
            )
