// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

[<EntryPoint>]
let main arg = 
    let result = Mesa.TableCreator.run(arg.[0], arg.[1])
    printfn "%A" result
    System.Console.ReadKey()
    0
