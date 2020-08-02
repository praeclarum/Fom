namespace Fom

open Microsoft.Build.Framework
open Microsoft.Build.Utilities

open System

type FomBuildTask () =
    inherit Task ()

    member val InputFiles : ITaskItem[] = [||] with get, set
    member val OutputPath : string = "" with get, set

    override this.Execute () =
        try
            printfn "%A" this.InputFiles

            let filesToConvert =
                this.InputFiles
                |> List.ofArray
                |> List.filter (fun x ->
                    x.ItemSpec.EndsWith("Domain.fs", StringComparison.CurrentCultureIgnoreCase))
                |> List.map (fun x -> IO.Path.GetFullPath x.ItemSpec)
            printfn "%A" filesToConvert

            let allTypes =
                filesToConvert
                |> List.collect TypeParser.parseAllTypes

            CodeGenerator.writeAllTypes Console.Out allTypes
            true
        with ex ->
            this.Log.LogErrorFromException(ex, false)
            false


