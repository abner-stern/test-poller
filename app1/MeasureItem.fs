module MeasureItem

open System
open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema

[<TableAttribute(name = "temperature_measurement")>]
[<CLIMutable>]
type MeasureItem =
    {
        [<Key>]
        [<Column(name = "last_updated")>]
        LastUpdated: DateTime

        [<Required>]
        [<Column(name = "temperature")>]
        Temperature: float
    }
