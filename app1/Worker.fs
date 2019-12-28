module Worker

type WorkService =
    abstract do_work: unit -> unit

type SimpleWorkService () =
    interface WorkService with
        member this.do_work () =
            printfn "do work."
            ()

