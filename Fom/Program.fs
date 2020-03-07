open System

module WishfulThinking =

    open TestDomain

    type PersonDiff =
        {
           NameDiff : string option
           AgeDiff : int option
        }
    let (-) (x : Person) (y: Person) =
            let localNameDiff = if x.Name <> y.Name then Some x.Name else None
            let localAgeDiff = if x.Age <> y.Age then Some x.Age else None
            if Option.isNone localNameDiff && Option.isNone localAgeDiff then None
            else Some {
                NameDiff = localNameDiff
                AgeDiff = localAgeDiff
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

    let diff = p2 - p1

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
