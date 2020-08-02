namespace TodoDomain

open System
open System.Numerics

type UserData =
    {
        Lists : TodoList[]
    }

and TodoList =
    {
        Title : string
        Items : TodoItem[]
        Color : Vector4
    }

and TodoItem =
    {
        Title : string
        Completed : DateTimeOffset option
        Important : bool
    }




