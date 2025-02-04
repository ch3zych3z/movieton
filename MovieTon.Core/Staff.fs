module MovieTon.Core.Staff

open System

type StaffMember = {
    id: int
    name: string
}
with
    static member Of id name = {
        id = id
        name = name
    }

type Role =
    | Actor
    | Actress
    | Director
with
    static member TryParse(str: string) =
        if "actor".Equals(str, StringComparison.InvariantCultureIgnoreCase) then
            Some Actor
        elif "actress".Equals(str, StringComparison.InvariantCultureIgnoreCase) then
            Some Actress
        elif "director".Equals(str, StringComparison.InvariantCultureIgnoreCase) then
            Some Director
        else None

    static member Parse(str: string) =
        Role.TryParse str |> Option.get

    override x.ToString() =
        match x with
        | Actor -> "actor"
        | Actress -> "actress"
        | Director -> "director"

type Participation = {
    staffId: int
    movieId: int
    role: Role
}
with
    static member Of staffId movieId role = {
        staffId = staffId
        movieId = movieId
        role = role
    }

    member x.IsDirector =
        match x.role with
        | Director -> true
        | _ -> false
