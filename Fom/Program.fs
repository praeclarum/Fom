open System

module WishfulThinking =

    open TestDomain

    type PersonDiff =
         {
            NameDiff : string option
            AgeDiff : int option
         }

    //type Person with
        //static member (-) (x: Person, y:Person) =
            //{
            //    NameDiff = if x.Name <> y.Name then Some x.Name else None
            //    AgeDiff = if x.Age <> y.Age then Some x.Age else None
            //}

    let (-) (x : Person) (y: Person) =
        {
            NameDiff = if x.Name <> y.Name then Some x.Name else None
            AgeDiff = if x.Age <> y.Age then Some x.Age else None
        }


    type Contact = 
        | PhoneNumber of string
        | Address of string
        | DontCall

    //type ContactDiff =
        //| PhoneNumberDiff of string option
        //| AddressDiff of string option
        //| DontCallDiff


    let p1 = { Name = "Frank"; Age = 39 }

    let p2 = { Name = "Frank"; Age = 40 }

    let diff : PersonDiff = p2 - p1

    //let p2_again = p1 + diff

    //match diff.Name with
    //| None -> () // I didn't change my name
    //|


[<EntryPoint>]
let main argv =

    match argv with
    | [||] ->
        printfn "Usage: fom code.fs"
        2
    | _ ->
        try
            let fileName = argv.[0]
            let fullPath = IO.Path.GetFullPath fileName

            //printfn "FOM! %A" fullPath

            let types = TypeParser.parseAllTypes fullPath

            //printfn "ALL TYPES %A" types

            CodeGenerator.writeAllTypes Console.Out types

            0 // return an integer exit code
        with ex ->
            printfn "ERROR! %O" ex
            1
