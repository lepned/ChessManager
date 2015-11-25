module ChessManager.Domain.Program

open ChessManager.Domain.Implementation

let players n = 
    [1..n]
    |>List.map (fun id -> {Id=id; Navn = sprintf "Spiller %i" id; Rating = 1200 + (id*100)})

let spillere = players 10

let getRunde () = 
    let runde = lagRunde spillere
    //printfn "%A" runde
    runde

let getRes runde = runde.Pairings |> List.map simRes

let update runde =
    for pairs in getRes runde do
        for (p,res) in pairs do
            updateState p res

let settOpp n =
    for runde in [1..n] do
        let runde = getRunde ()
        let res = getRes runde
        update runde
            
settOpp 9

let runde = api.Settopp spillere
let stat = api.Stat spillere

let resultatene = res
let score = spillere |> List.map calcScore
let fargeStat = spillere |> List.map calcFargeStat
fargeStat |> List.iter (printfn "%A")

System.Console.ReadLine() |> ignore

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    0 // return an integer exit code
