namespace Fom

open Microsoft.Build.Framework
open Microsoft.Build.Utilities

open System

type FomBuildTask () =
    inherit Task ()

    member val InputFiles : ITaskItem[] = [||] with get, set

    override this.Execute () =
        try
            printfn "%A" this.InputFiles

            let filesToConvert =
                this.InputFiles
                |> List.ofArray
                |> List.filter (fun x ->
                    x.ItemSpec.EndsWith("Domain.fs", StringComparison.CurrentCultureIgnoreCase))
                |> List.map (fun x -> IO.Path.GetFullPath x.ItemSpec)
            //printfn "%A" filesToConvert

            for fileToConvert in filesToConvert do

                let types = TypeParser.parseAllTypes fileToConvert

                let outputDir = IO.Path.GetDirectoryName fileToConvert
                let outputName = IO.Path.GetFileNameWithoutExtension fileToConvert + "Diff.fs"
                let outputPath = IO.Path.Combine (outputDir, outputName)
                use output = new IO.StreamWriter (outputPath)
                
                CodeGenerator.writeAllTypes output types
            false
        with ex ->
            this.Log.LogErrorFromException(ex, false)
            false


