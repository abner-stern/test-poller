open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Configuration.CommandLine
open Microsoft.Extensions.Configuration.EnvironmentVariables
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open System

let build_logging ctx (logging:ILoggingBuilder) =
    logging.AddConsole() |> ignore

let build_config (argv: string []) ctx (config:IConfigurationBuilder) =
    config
      .AddJsonFile("appsettings.json", optional = true)
      .AddEnvironmentVariables(prefix = "PREFIX_")
      .AddCommandLine(argv) |> ignore

let dump_config (ctx:HostBuilderContext) =
    let children =
        ctx.Configuration.GetChildren()
        |> List.ofSeq
        |> List.map (fun x -> ("path", x.Path, "key", x.Key, "val", x.Value))
    let str = List.map (fun x -> sprintf "%A, " x) children
    printfn "config: %s" (str.ToString())

let configure_services ctx (services: IServiceCollection) =
    dump_config ctx
    Srv.configure ctx services

[<EntryPoint>]
let main argv =
    printfn "Starting the app."
    let host =
        HostBuilder()
           .ConfigureAppConfiguration(fun ctx cfg -> build_config argv ctx cfg)
           .ConfigureLogging(build_logging)
           .ConfigureServices(configure_services)
           .Build()
           .Run()
    printfn "Stopping the app."
    0 // return an integer exit code
