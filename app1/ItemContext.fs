module ItemContext

open Microsoft.EntityFrameworkCore

open Item

type ItemContext (options: DbContextOptions<ItemContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable items: DbSet<Item>
    member __.Items
        with get() = __.items
        and set value = __.items <- value
