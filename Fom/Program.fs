open System

module WishfulThinking =

    type Person =
        {
            Name : string
            Age : int
        }

    type ContactInfo = 
        | PhoneNumber of string
        | Address of string
        | DontCall

    let p1 = { Name = "Frank"; Age = 39 }

    let p2 = { Name = "Frank"; Age = 40 }

    //let diff = p2 - p1

    //match diff.Name with
    //| None -> () // I didn't change my name
    //|




module InputData =

    type FomType =
        {
            Name : string
            Namespace : string
            Body : TypeBody
        }

    and TypeBody =
        | StructBody of Members : StructMember[]
        | EnumBody of Members : EnumMember[]

    and StructMember = {
        Name : string; MemberType : string
    }
    and EnumMember = {
        Name : string; MemberType : string option
    }

open InputData

module TestData =

    let personType =
        {
            Name = "Person"
            Namespace = "WishfulThinking"
            Body = StructBody [|
                { Name = "Name"; MemberType = "string" }
                { Name = "Age"; MemberType = "int" }
            |]
        }
    let contactType =
        {
            Name = "Contact"
            Namespace = "WishfulThinking"
            Body = EnumBody [|
                { Name = "PhoneNumber"; MemberType = Some "string" }
                { Name = "Address"; MemberType = Some "int" }
                { Name = "DontCall"; MemberType = None }
            |]
        }


module CodeGenerator =

    let w = Console.Out

    let t = TestData.personType

    let writeDiffType (w : IO.TextWriter) (t : FomType) : unit =

        let writeStructMem (m : StructMember) =
            w.WriteLine ("       {0} : {1}", m.Name, m.MemberType)
        let writeEnumMem (m : EnumMember) =
            let line =
                match m.MemberType with
                | Some x -> sprintf "    | %s of %s" m.Name x
                | None -> sprintf "    | %s" m.Name
            w.WriteLine (line)

        w.WriteLine ("type {0} =", t.Name)

        match t.Body with
        | StructBody members ->
            w.WriteLine "    {"
            members |> Seq.iter writeStructMem
            w.WriteLine "    }"
        | EnumBody members ->
            members |> Seq.iter writeEnumMem

    writeDiffType w TestData.personType


















[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
