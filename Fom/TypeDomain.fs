module TypeDomain

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

type FomModule =
    {
        Opens : string list
        Types : FomType list
    }



