open System

module WishfulThinking =

    type Person =
        {
            Name : string
            Age : int
        }

    let p1 = { Name = "Frank"; Age = 39 }

    let p2 = { Name = "Frank"; Age = 40 }

    let diff = p2 - p1

    match diff.Name with
    | None -> () // I didn't change my name
    |



































[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
