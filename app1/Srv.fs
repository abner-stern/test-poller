module Srv

open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Http

open App
open ItemContext
open Poller
open Worker

let configure_db (ctx: HostBuilderContext) (opts: DbContextOptionsBuilder) =
    let conn_str = ctx.Configuration.GetSection("ConnectionStrings").GetSection("ItemContext").Value
    opts.UseNpgsql conn_str |> ignore

let configure (ctx: HostBuilderContext) (services: IServiceCollection) =
    services.AddHostedService<Capp> () |> ignore
    services.AddDbContext<ItemContext> (fun opts -> configure_db ctx opts) |> ignore
    services.AddHttpClient () |> ignore
    services.AddTransient<WorkService, SimpleWorkService> () |> ignore
    services.AddHostedService<Poller> () |> ignore
