module Item

open System.ComponentModel.DataAnnotations.Schema

[<TableAttribute(name = "item")>]
[<CLIMutable>]
type Item =
    {
        [<Column(name = "id")>]
        Id: int
        [<Column(name = "user_id")>]
        UserId: int
        [<Column(name = "name")>]
        Name: string
    }
