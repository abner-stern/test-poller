module ItemContext

open Microsoft.EntityFrameworkCore

open Item
open MeasureItem

type ItemContext (options: DbContextOptions<ItemContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable items: DbSet<Item>
    member __.Items
        with get() = __.items
        and set value = __.items <- value

    [<DefaultValue>]
    val mutable measure_items: DbSet<MeasureItem>
    member __.MeasureItems
        with get() = __.measure_items
        and set value = __.measure_items <- value

