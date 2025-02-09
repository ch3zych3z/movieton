module MovieTon.Utils.Collections

open System.Collections.Generic

module Dictionary =

    let init2<'a, 'b when 'a: equality> (keys: 'a seq) (values: 'b seq) =
        Seq.map2 (fun key value -> KeyValuePair(key, value)) keys values
        |> Dictionary

    let init2Map<'a, 'b, 'c when 'a: equality> (mapper: 'a -> 'b -> 'c) (keys: 'a seq) (values: 'b seq) =
        Seq.map2 (fun key value -> KeyValuePair(key, mapper key value)) keys values
        |> Dictionary

    let init3Map<'a, 'b, 'c, 'd when 'a: equality>
        (mapper: 'a -> 'b -> 'c -> 'd)
        (keys: 'a seq)
        (values1: 'b seq)
        (values2: 'c seq)
        =
        Seq.map3 (fun key value1 value2 -> KeyValuePair(key, mapper key value1 value2)) keys values1 values2
        |> Dictionary

    let getOrUpdate<'a, 'b> (key: 'a) (defaultValue: 'b) (d: Dictionary<'a, 'b>) =
        let contains, value = d.TryGetValue(key)
        if contains then value
        else
            d[key] <- defaultValue
            defaultValue

    let getOrCompute<'a, 'b> (key: 'a) (computeDefault: unit -> 'b) (d: Dictionary<'a, 'b>) =
        let contains, value = d.TryGetValue(key)
        if contains then value
        else
            let defaultValue = computeDefault ()
            d[key] <- defaultValue
            defaultValue

    let get<'a, 'b> (key: 'a) (d: Dictionary<'a, 'b>) =
        let contains, value = d.TryGetValue(key)
        if contains then Some value
        else None

    let mapValues<'a, 'b, 'c when 'a: equality> (mapper: 'b -> 'c) (d: Dictionary<'a, 'b>) =
        seq {
            for KeyValue(key, value) in d do
                yield KeyValuePair(key, mapper value)
        } |> Dictionary
