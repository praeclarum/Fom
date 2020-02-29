open System

module WishfulThinking =

    type Person =
        {
            Name : string
            Age : int
        }

    type PersonDiff =
        {
            Name : string option
            Age : int option
        }

    type Contact = 
        | PhoneNumber of string
        | Address of string
        | DontCall

    //type ContactDiff =
        //| PhoneNumberDiff of string option
        //| AddressDiff of string option
        //| DontCallDiff


    //let p1 = { Name = "Frank"; Age = 39 }

    //let p2 = { Name = "Frank"; Age = 40 }

    //let diff : PersonDiff = p2 - p1

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
            w.WriteLine ("       {0} : {1} option", m.Name, m.MemberType)
        let writeEnumMem (m : EnumMember) =
            let line =
                match m.MemberType with
                | Some x -> sprintf "    | %s of %s option" m.Name x
                | None -> sprintf "    | %s" m.Name
            w.WriteLine (line)

        w.WriteLine ("type {0}Diff =", t.Name)

        match t.Body with
        | StructBody members ->
            w.WriteLine "    {"
            members |> Seq.iter writeStructMem
            w.WriteLine "    }"
        | EnumBody members ->
            members |> Seq.iter writeEnumMem

    writeDiffType w TestData.personType
    writeDiffType w TestData.contactType




    #r "/Users/fak/.nuget/packages/fsharp.compiler.service/32.0.0/lib/net461/FSharp.Compiler.Service.dll"

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

        let convertModuleToInputData (moduleId : LongIdent) (decls : SynModuleDecls) : InputData.FomType list =
            printfn "FOUND MODULE %A" moduleId
            []

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

        let path = "/Users/fak/Dropbox/Projects/Fom/Fom/Program.fs"
        let ast = parse path

        convertAstToInputData ast

        ()















[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
