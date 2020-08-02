//#r "/Users/fak/.nuget/packages/fsharp.compiler.service/32.0.0/lib/net461/FSharp.Compiler.Service.dll"

module TypeParser

open System

open TypeDomain

open FSharp.Compiler
open FSharp.Compiler.Ast

let private parse (path : string) =
    let checker = SourceCodeServices.FSharpChecker.Create ()
    let code = IO.File.ReadAllText path
    let sourceText = FSharp.Compiler.Text.SourceText.ofString code
    let poptions =
        { SourceCodeServices.FSharpParsingOptions.Default
            with
             SourceFiles = [| path |]
        }
    let parseResult =
        let fileName = IO.Path.GetFileName path
        checker.ParseFile (fileName, sourceText, poptions)
        |> Async.RunSynchronously
    parseResult.ParseTree

let private stringIdent (moduleId : LongIdent) =
    String.Join (".", moduleId)

let private stringIdentWithDots (moduleId : LongIdentWithDots) =
    match moduleId with
    | LongIdentWithDots (ids, _) ->
        stringIdent ids

let rec private synTypeToString (typ : SynType) : string =
    match typ with
    | SynType.LongIdent (LongIdentWithDots (id, _)) ->
        stringIdent id
    | SynType.Array (1, innerType, _) ->
        synTypeToString innerType + "[]"
    | SynType.App (SynType.LongIdent (LongIdentWithDots ([typeIdent],[])), _, [typeArg], _, _, true, _) ->
        let arg = synTypeToString typeArg
        let r = sprintf "%s %O" arg typeIdent
        r
    | _ ->
        ()
        failwithf "I don't know how to deal with %A (%O)" typ (typ.GetType())

let rec private convertModuleToInputData (moduleId : LongIdent) (decls : SynModuleDecls) : FomType list =
    //printfn "FOUND MODULE %A" moduleId
    let moduleNamespace = stringIdent moduleId
    decls
    |> List.collect (function
        | SynModuleDecl.Types (types, _) -> convertTypeToInputData moduleNamespace types
        | _ -> [])

and private convertTypeToInputData (moduleNamespace : string) (types : SynTypeDefn list) =
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
                    fun (SynField.Field (id,_,name, fieldType, z, _, _, _)) ->
                        let name =
                            match name with
                            | Some x -> string x
                            | None -> "IDontKnow"
                        let fieldType =
                            synTypeToString fieldType
                        let r:RecordMember option =
                            Some { Name = name;
                                   MemberType = fieldType }
                        r)
                |> Array.ofSeq
            [{
                Name = typeName
                Namespace = moduleNamespace
                Body = RecordBody fields
            }]
        | _ -> [])


let private convertAstToInputData (ast : Ast.ParsedInput option) : FomModule =
    match ast with
    | Some (ParsedInput.ImplFile (ParsedImplFileInput (_,_,_,_,_,mods,_))) ->
        let opens =
            mods
            |> List.collect (fun (SynModuleOrNamespace(modId,isRec,isModule,decls,xmlDoc,attribs,access,range)) ->
                decls
                |> List.collect (function
                    | SynModuleDecl.Open (ident, _) ->
                        [stringIdentWithDots ident]
                    | x ->
                        []))
        let types =
            mods
            |> List.collect (fun (SynModuleOrNamespace(modId,isRec,isModule,decls,xmlDoc,attribs,access,range)) ->
                decls
                |> List.collect (function
                    | SynModuleDecl.Types (types, _) ->
                        let moduleNamespace = stringIdent modId
                        convertTypeToInputData moduleNamespace types
                    | SynModuleDecl.NestedModule (SynComponentInfo.ComponentInfo (_,_,_,mId,_,_,_,_),_,decls,_,_) ->
                        convertModuleToInputData mId decls 
                    | _ -> []))
        { Opens = opens; Types = types }
    | _ -> { Opens = []; Types = [] }


let parseAllTypes path =
    let ast = parse path
    convertAstToInputData ast





