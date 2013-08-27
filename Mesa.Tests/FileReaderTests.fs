namespace Mesa.Tests

open Mesa
open Fixie
open Shouldly

type ParserTests() = 

    member this.should_parse_sample1 =
        let filename = "sample1.csv"

        let parser = new Mesa.Parser()

        let rows = parser.records(',', filename)

        for r in rows do
            System.Console.WriteLine r
            for c in r do
                System.Console.WriteLine c
