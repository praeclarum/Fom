


## What is FOM?

Fom is a code generation tool to people using a funcitonal style
of programming to update user interfaces.

It achieves this by making it easy to create "diffs" between
two immutable objects.

For example, itf we have this type

```fsharp
    type Person =
        {
            Name : string
            Age : int
        }

```

FOM makes it possible to create diffs of two `Person` objects to
tell what changed. This is useful when writing data-backed UIs.

For example, if I download two `Person` objects, I can tell the difference:

```fsharp
    let person1 = { Name = "Frank"; Age = 39 }

    let person2 = { Name = "Frank"; Age = 40 }

    let whatChanged = person2 - person1
```

Now, `whatChanged` will have all `None`-fields except the `AgeDiff` field
will have the new value.

Then diff looks like this:

```fsharp
    type PersonDiff =
         {
            NameDiff : string option
            AgeDiff : string option
         }
         static member (-) (x : Person, y: Person) =
             {
                 NameDiff = if x.Name <> y.Name then Some x.Name else None
                 AgeDiff = if x.Age <> y.Age then Some x.Age else None
             }
```









