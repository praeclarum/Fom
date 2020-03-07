module TestDomain

type Person =
    {
        Name : string
        Age : int
    }

type Contact = 
    | PhoneNumber of string
    | Address of string
    | DontCall

