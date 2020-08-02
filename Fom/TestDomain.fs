module TestDomain

type Person =
    {
        Name : string
        Age : int
        Contacts : Contact[]
        Partner : Person option
    }

and Contact = 
    | PhoneNumber of string
    | Address of string
    | DontCall

