module CodeGenerator

open System
open TypeDomain

let w = Console.Out


module TestData =

    open TypeDomain

    let personType =
        {
            Name = "Person"
            Namespace = "WishfulThinking"
            Body = RecordBody [|
                { Name = "Name"; MemberType = "string" }
                { Name = "Age"; MemberType = "int" }
            |]
        }
    let contactType =
        {
            Name = "Contact"
            Namespace = "WishfulThinking"
            Body = UnionBody [|
                { Name = "PhoneNumber"; MemberType = Some "string" }
                { Name = "Address"; MemberType = Some "int" }
                { Name = "DontCall"; MemberType = None }
            |]
        }


let private writeDiffType (w : IO.TextWriter) (t : FomType) : unit =

    let writeStructMem (m : RecordMember) =
        w.WriteLine ("           {0}Diff : {1} option", m.Name, m.MemberType)

    let writeRecordDiff (ms : RecordMember[]) =
        w.WriteLine ("    let (-) (x : {0}, y: {0}) =", t.Name)
        w.WriteLine ("        {")
        for m in ms do
            w.WriteLine ("            {0}Diff = if x.{0} <> y.{0} then Some x.{0} else None", m.Name, m.Name)
        w.WriteLine ("        }")

    let writeEnumMem (m : UnionMember) =
        let line =
            match m.MemberType with
            | Some x -> sprintf "        | %s of %s option" m.Name x
            | None -> sprintf "        | %s" m.Name
        w.WriteLine (line)
    w.WriteLine ("    type {0}Diff =", t.Name)

    match t.Body with
    | RecordBody members ->
        w.WriteLine "        {"
        members |> Seq.iter writeStructMem
        w.WriteLine "        }"
        writeRecordDiff members
    | UnionBody members ->
        members |> Seq.iter writeEnumMem

    

//writeDiffType w TestData.personType
//writeDiffType w TestData.contactType

//allTypes
//|> Seq.groupBy (fun x -> x.Namespace)
//|> Seq.iter (fun (m, types) ->
    //w.WriteLine ("module {0} = ", m)
    //types |> Seq.iter (writeDiffType Console.Out))


let writeAllTypes (w : IO.TextWriter) allTypes = 
    allTypes
    |> Seq.groupBy (fun x -> x.Namespace)
    |> Seq.iter (fun (m, types) ->
        w.WriteLine ("module {0} = ", m)
        types |> Seq.iter (writeDiffType w))


