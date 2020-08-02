namespace TodoDomain
open System
open System.Numerics
type UserDataDiff =
    {
           ListsDiff : TodoList[] option
    }
[<AutoOpen>]
module UserDataDiffOps =
    let (-) (x : UserData) (y: UserData) =
        let localListsDiff = if x.Lists <> y.Lists then Some x.Lists else None
        if Option.isNone localListsDiff then None
        else Some {
            ListsDiff = localListsDiff
        }
type TodoListDiff =
    {
           TitleDiff : string option
           ItemsDiff : TodoItem[] option
           ColorDiff : Vector4 option
    }
[<AutoOpen>]
module TodoListDiffOps =
    let (-) (x : TodoList) (y: TodoList) =
        let localTitleDiff = if x.Title <> y.Title then Some x.Title else None
        let localItemsDiff = if x.Items <> y.Items then Some x.Items else None
        let localColorDiff = if x.Color <> y.Color then Some x.Color else None
        if Option.isNone localTitleDiff && Option.isNone localItemsDiff && Option.isNone localColorDiff then None
        else Some {
            TitleDiff = localTitleDiff
            ItemsDiff = localItemsDiff
            ColorDiff = localColorDiff
        }
type TodoItemDiff =
    {
           TitleDiff : string option
           CompletedDiff : DateTimeOffset option option
           ImportantDiff : bool option
    }
[<AutoOpen>]
module TodoItemDiffOps =
    let (-) (x : TodoItem) (y: TodoItem) =
        let localTitleDiff = if x.Title <> y.Title then Some x.Title else None
        let localCompletedDiff = if x.Completed <> y.Completed then Some x.Completed else None
        let localImportantDiff = if x.Important <> y.Important then Some x.Important else None
        if Option.isNone localTitleDiff && Option.isNone localCompletedDiff && Option.isNone localImportantDiff then None
        else Some {
            TitleDiff = localTitleDiff
            CompletedDiff = localCompletedDiff
            ImportantDiff = localImportantDiff
        }
