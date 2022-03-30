namespace ClassLibrary1

open System.Text.Json
open Newtonsoft.Json

type FUnion =
    | A of int
    | B of string

module Test =
let a () =
    let option = A 5
    //JsonSerializer.Serialize option
    JsonConvert.SerializeObject option