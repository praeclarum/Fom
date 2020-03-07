open System

module WishfulThinking =

    type Person =
        {
            Name : string
            Age : int
        }

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




module TypeDomain =

    type FomType =
        {
            Name : string
            Namespace : string
            Body : TypeBody
        }

    and TypeBody =
        | RecordBody of Members : RecordMember[]
        | UnionBody of Members : UnionMember[]

    and RecordMember = {
        Name : string; MemberType : string
    }
    and UnionMember = {
        Name : string; MemberType : string option
    }

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


module CodeGenerator =

    open System
    open TypeDomain

    let w = Console.Out

    let t = TestData.personType

    let writeDiffType (w : IO.TextWriter) (t : FomType) : unit =

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

        

    writeDiffType w TestData.personType
    writeDiffType w TestData.contactType

    //allTypes
    //|> Seq.groupBy (fun x -> x.Namespace)
    //|> Seq.iter (fun (m, types) ->
        //w.WriteLine ("module {0} = ", m)
        //types |> Seq.iter (writeDiffType Console.Out))






//#r "/Users/fak/.nuget/packages/fsharp.compiler.service/32.0.0/lib/net461/FSharp.Compiler.Service.dll"

open FSharp.Compiler
open FSharp.Compiler.Ast

module DataLoader =

    let parse (path : string) =
        let checker = SourceCodeServices.FSharpChecker.Create ()
        let code = IO.File.ReadAllText path
        let sourceText = FSharp.Compiler.Text.SourceText.ofString code
        let poptions =
            { SourceCodeServices.FSharpParsingOptions.Default
                with
                 SourceFiles = [| path |]
            }
        let parseResult =
            checker.ParseFile ("foo.fs", sourceText, poptions)
            |> Async.RunSynchronously
        parseResult.ParseTree

    let path = "/Users/fak/Dropbox/Projects/Fom/Fom/Program.fs"
    let ast = parse path

    let stringIdent (moduleId : LongIdent) =
        String.Join (".", moduleId)

    let convertModuleToInputData (moduleId : LongIdent) (decls : SynModuleDecls) : InputData.FomType list =
        printfn "FOUND MODULE %A" moduleId
        let moduleNamespace = stringIdent moduleId
        decls
        |> List.collect (function
            | SynModuleDecl.Types (types, _) ->
                types
                |> List.collect (fun (x : SynTypeDefn) ->
                    match x with
                    | TypeDefn (SynComponentInfo.ComponentInfo (_,_,_,tId,_,_,_,_),
                                SynTypeDefnRepr.Simple (SynTypeDefnSimpleRepr.Record (_, fields : SynField list, _), _),
                                _, _) ->
                        let typeName = stringIdent tId
                        let fields : RecordMember[] =
                            fields
                            |> Seq.choose (
                                function
                                | SynField.Field (id,_,name, y, z, _, _, _) ->
                                    let name =
                                        match name with
                                        | Some x -> string x
                                        | None -> "IDontKnow"
                                    let r:StructMember option =
                                        Some { Name = name;
                                               MemberType = "string" }
                                    r
                                | _ -> None)
                            |> Array.ofSeq
                        [{
                            Name = typeName
                            Namespace = moduleNamespace
                            Body = StructBody fields
                        }]
                    | _ -> [])
            | _ -> [])

    let convertAstToInputData (ast : Ast.ParsedInput option) : InputData.FomType list =
        match ast with
        | Some (ParsedInput.ImplFile (ParsedImplFileInput (_,_,_,_,_,mods,_))) ->
            mods
            |> List.collect (fun (SynModuleOrNamespace(_,isRec,isModule,decls,xmlDoc,attribs,access,range)) ->
                decls
                |> List.collect (function
                    | SynModuleDecl.NestedModule (SynComponentInfo.ComponentInfo (_,_,_,mId,_,_,_,_),_,decls,_,_) ->
                        convertModuleToInputData mId decls 
                    | _ -> []))
        | _ -> List.empty


    let allTypes = convertAstToInputData ast

    let w = Console.Out

    allTypes
    |> Seq.groupBy (fun x -> x.Namespace)
    |> Seq.iter (fun (m, types) ->
        w.WriteLine ("module {0} = ", m)
        types |> Seq.iter (CodeGenerator.writeDiffType Console.Out))

    ()















[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
