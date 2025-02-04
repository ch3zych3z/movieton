module MovieTon.CLIRunner.Interpreter

open MovieTon.Core.App
open MovieTon.Core.Movie
open MovieTon.Core.Staff
open MovieTon.Core.Tag
open MovieTon.Core.Info

type InfoInst =
    | MovieInfo of string
    | StaffInfo of string
    | TagInfo of string

type PutAllInst =
    | PutStaffMembers of StaffMember seq
    | PutParticipation of Participation seq
    | PutMovies of Movie seq
    | PutTitles of Title seq
    | PutTags of Tag seq
    | PutMovieTags of MovieTag seq

type ParseInst<'a> = {
    parse: unit -> 'a
    doAfter: 'a -> Instruction seq
}

and Instruction =
    | Exit of int
    | Print of string
    | Info of InfoInst
    | Put of PutAllInst
    | Parse of ParseInst<obj>

type Interpreter(mtApp: MovieTonApp) =

    let errorIter msg v f =
        match v with
        | Some v -> f v
        | None -> printfn $"{msg}"

    let printWithTabs (tabs: int) obj =
        for i in 1..tabs do
            printf "\t"
        printfn $"{obj}"

    let printWithTabsSeq (tabs: int) objs =
        for i in 1..tabs do
            printf "\t"
        match Seq.tryHead objs with
        | Some obj ->
            printf $"{obj}"
            for obj in Seq.tail objs do
                printf $", {obj}"
        | None -> ()
        printfn ""

    let ppStaffView tabSize (view: StaffInfo) =
        printWithTabs tabSize view.name

    let ppMovieView tabSize (view: MovieInfo) =
        printWithTabs tabSize $"\"{view.title}\":"
        printWithTabs (tabSize + 1) $"Rating: {view.rating / 10}.{view.rating % 10}"

        let directorName =
            match view.director with
            | Some director -> director.name
            | _ -> "Unknown"
        printWithTabs (tabSize + 1) $"Director: {directorName}"
        printWithTabs (tabSize + 1) "Actors:"
        for actor in view.actors do
            ppStaffView (tabSize + 2) actor

        printWithTabs (tabSize + 1) "Tags:"
        printWithTabsSeq (tabSize + 2) view.tags

    let print inst =
        match inst with
        | MovieInfo title ->
            let movie = mtApp.GetMovieInfo(title)
            ppMovieView 0
            |> errorIter $"No movie found with title: \"{title}\"" movie
        | StaffInfo name ->
            let movies = mtApp.GetStaffInfo(name)
            errorIter $"No movie found for {name}" movies (fun movies ->
                printfn $"{name} is involved in movies:"
                Seq.iter (ppMovieView 1) movies
            )
        | TagInfo tagName ->
            let movies = mtApp.GetTagMovies(tagName)
            errorIter $"No movie found for {tagName}" movies (fun movies ->
                printfn $"Movies with tag {tagName}:"
                Seq.iter (ppMovieView 1) movies
            )

    let doPutInst inst =
        match inst with
        | PutStaffMembers staff -> mtApp.PutStaffMembers staff
        | PutParticipation participation -> mtApp.PutParticipation participation
        | PutMovies movies -> mtApp.PutMovies movies
        | PutTitles titles -> mtApp.PutTitles titles
        | PutTags tags -> mtApp.PutTags tags
        | PutMovieTags movieTags -> mtApp.PutMovieTags movieTags

    member private x.ParseInst inst =
        let parsed = inst.parse ()
        let mutable code = 0
        for inst in inst.doAfter parsed do
            if code = 0 then
                code <- x.Interpret inst
        code

    member x.Interpret(inst: Instruction): int =
        match inst with
        | Exit code -> code
        | Print str -> printfn $"{str}"; 0
        | Info inst -> print inst; 0
        | Put inst -> doPutInst inst; 0
        | Parse inst -> x.ParseInst(inst)
