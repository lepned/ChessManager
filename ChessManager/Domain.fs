namespace ChessManager.Domain

type Farge = Sort | Hvit
type Player = {Id:int; Navn:string; Rating:int}
type Players = Player list
type Pair = {Hvit:Player; Sort:Player}
type Bord = {Id:int; Par:Pair}
type Pairings = Bord list
type Runde = {Runde:int; Pairings:Pairings}
type MittResultat = Vant | Tapte | Remis | Wo
type BordResultat = {MinFarge:Farge; Motst: Player; Resultat:MittResultat}
type FargeStat = {Player:Player;AntHvite:int; AntSorte:int; InARow:int; Siste: Farge option}

type RundeCapability = Players -> Runde
type Report = Players -> FargeStat list

type ChessManagerAPI =
    {
        Settopp : RundeCapability
        Stat : Report
    }



