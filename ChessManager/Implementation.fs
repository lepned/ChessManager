module ChessManager.Domain.Implementation

//type Farge = Sort | Hvit
//type Player = {Id:int; Navn:string; Rating:int}
//type Players = Player list
//type Pair = {Hvit:Player; Sort:Player}
//type Bord = {Id:int; Par:Pair}
//type Pairings = Bord list
//type Runde = {Runde:int; Pairings:Pairings}
//type MittResultat = Vant | Tapte | Remis | Wo
//type BordResultat = {MinFarge:Farge; Motst: Player; Resultat:MittResultat}

type Resultater = System.Collections.Generic.Dictionary<Player, BordResultat list>
let res = Resultater()
let rnd = System.Random()
let finnRunde players =
    let ant player =
        match res.TryGetValue player with
        |true, li -> (li |> List.length) + 1
        |_ -> 1
    let player = players |> List.maxBy ant
    ant player

let lagRunde players  = 
    let runde = finnRunde players
    let rec loop = function
        |[] -> []
        |a::b::t -> 
            let utfall = a::b::[]
            let hvit = utfall.[rnd.Next(0,utfall.Length)]
            let sort = utfall|>List.find(fun e -> e <> hvit)
            (hvit,sort)::loop t
        |_ -> failwith "Pairing not possible"
    let pairings = 
        loop players 
        |> List.mapi (fun idx (a,b) -> {Id=idx + 1 ; Par = {Hvit=a;Sort=b}})
    {Runde=runde; Pairings=pairings}

let updateState player bordRes =
    match res.TryGetValue player with
    |true, li -> res.[player] <- bordRes::li
    |_ -> res.[player] <- [bordRes]

let simRes {Id=id; Par=par} =
    let {Hvit=hvit; Sort=sort} = par
    let utfall = [Vant;Tapte;Remis]
    let resHvit = utfall.[rnd.Next(0,utfall.Length)]
    let resSort = 
        if resHvit = Vant then Tapte
        elif resHvit = Tapte then Vant
        else Remis
    (hvit,{MinFarge=Hvit;Motst=sort; Resultat=resHvit})::
    (sort,{MinFarge=Sort;Motst=hvit; Resultat=resSort})::[]

let calcScore player =
    match res.TryGetValue player with
    |true, li -> 
        li |> List.sumBy (fun el -> 
            match el.Resultat with
            |Wo | Vant -> 1.0
            |Tapte -> 0.0
            |Remis -> 0.5)
    |_ -> 0.0

let calcInArow player =
    let state = {Player=player; AntHvite=0; AntSorte=0; InARow=0; Siste=None}
    let rec loop (list:BordResultat list) acc =
        match list with
        |[] -> acc
        |h::t ->
            if acc = state then
                loop t {acc with InARow=acc.InARow + 1; Siste=Some h.MinFarge}
            else 
                if acc.Siste = Some h.MinFarge then
                    loop t {acc with InARow=acc.InARow + 1}
                else acc
    match res.TryGetValue player with
    |true, li -> loop li state
    |_ -> state

let calcFarger player =
    match res.TryGetValue player with
    |true, li -> 
        let hvite =
            li |> List.sumBy (fun el -> 
            match el.MinFarge with
            |Hvit -> 1
            |Sort -> 0)
        let sorte =
            li |> List.sumBy (fun el -> 
            match el.MinFarge with
            |Hvit -> 0
            |Sort -> 1)
        hvite,sorte
    |_ -> 0,0

let calcFargeStat player =
    let hvite,sorte = calcFarger player
    let inRow = calcInArow player
    {inRow with AntHvite=hvite; AntSorte=sorte}

let api =
    {
        Settopp = fun players -> lagRunde players 
        Stat = fun players -> players |> List.map calcFargeStat
    }