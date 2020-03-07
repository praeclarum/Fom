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
    printfn "Hello World from F#!"
    0 // return an integer exit code
