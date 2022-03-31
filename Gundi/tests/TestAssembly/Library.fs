namespace TestAssembly

open System
open Microsoft.FSharp.Core

type FRecord = { X: int; Y: string }
type MyFsharpUnion =
    | A of int
    | B of string
    | F of FRecord
    | T of string * int
    
type BaseException(unionType: Type, expectedCase: string, actualCase: string) =
    inherit Exception($"Wrong %s{unionType.Name} cast. Expected: %s{expectedCase}, Actual: %s{actualCase}")

type MyException(unionType: Type, expectedCase: string, actualCase: string) =
    inherit BaseException(unionType, expectedCase, actualCase)
