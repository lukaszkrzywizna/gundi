# gundi [![GitHub](https://img.shields.io/github/license/lukaszkrzywizna/gundi)](/LICENSE)

A generator for discriminated union for C# (based on union in F#)

- [Overview](#overview)
    - [Defining a union](#defining-a-union)
    - [Using](#using)
    - [Union casting](#union-casting)
    - [Serialization](#serialization)
- [License](#license)

## Overview

A union is a value that represents several different cases of a different name or/and type. It is useful for modeling
more complex choice types - to express that provided data could be shaped differently. For example, Union `TxtOrNumber`
of a `string txt` and `int number` informs, that type can contain a `string` or `int`, but never both.

### Defining a union

In order to use a generator, there has to be a defined simple union schema:

```c#
using Gundi;

namespace MyNamespace

[Union] // Mandatory attribute
public partial record SimpleUnion
{
    static partial void Cases(int a, string b, decimal c, int? d); // Mandatory function. `static partial void Cases` is a must-have
}
```

The `Union` attribute applies for a partial record (struct and class is NOT ALLOWED). It helps the generator, to
identify which types should be enhanced. The `Cases` method uses specified types and argument names to define union
cases.

The generator will generate a partial record which will contain all arguments kept as a private field and some public
API:

```c#

namespace Gundi.Tests
{
    partial record SimpleUnion
    {
        // tag which contain info about chosen  case
        private readonly byte tag;
        
        // private "case" fields
        private readonly System.Int32 a;
        private readonly System.String b;
        private readonly System.Decimal c;
        private readonly System.Int32? d;
        
        // public API like Map and Match methods
        // (..)
    }
}
```

### Using

Generated union should contain static argument-named factory function, and simple `match` & `map` methods:

```c#
var union = SimpleUnion.A(5);
Console.WriteLine(union.IsA()); // prints true
Console.WriteLine(union.IsB()); // prints false

var case = union.Match(a => a.ToString(), b => b, c => c.ToString(), d => d.ToString());
Console.WriteLine(case); // prints 5

var mapped = union.Map(
    a => a + 1,
    b => b[..2],
    c => c + 3,
    d => d + 4);
    
var mappedCase = mapped.Match(a => a.ToString(), b => b, c => c.ToString(), d => d.ToString());
Console.WriteLine(mappedCase); // prints 6

```

### Union casting

The generator will generate `Cast` functions, which "force" to get a defined union case or throws an exception. By
default, `InvalidOperationException` is thrown, but there is a possibility to override the type
with `CustomCastException` setting:

```c#
[Union(CustomCastException = typeof(MyException))]
public partial record UnionWithCustomException
{
    static partial void Cases(int a, string b);
}
```

The selected type must be an exception with a constructor with three arguments

```c#
public class MyException : Exception
{
    public MyException(
        Type unionType,      // First argument with union type.
        string expectedCase, // Second with a name of an expected case.
        string actualCase)   // Third with a name of a actual case.
        : base($"Wrong {unionType.Name} cast. Expected: {expectedCase}, Actual: {actualCase}")
    {
    }
}
```

### Serialization

Due to generating fields and constructor with `private` modifier, the record can't be deserialized as it is. To resolve this, `Gundi` provides custom JSON converters which are registered via `JsonConverter` attribute by default:

```c#
[System.Text.Json.Serialization.JsonConverter(typeof(UnionJsonConverterFactory))] // assigned by default
[Newtonsoft.Json.JsonConverter(typeof(UnionJsonNetConverter))] // assigned by default
public partial record SimpleUnion
```

```c#
using System.Text.Json;
using Newtonsoft.Json;

// (...)

var union = SimpleUnion.A(5);

// System.Text.Json:
var json = JsonSerializer.Serialize(union);
Console.WriteLine(json); // prints {"Case":"A","Fields":[5]}

var deserialized = JsonSerializer.Deserialize<SimpleUnion>(json);
Console.WriteLine(union == deserialized); // prints true

// Newtonsoft.Json:
var jsonNet = JsonConvert.SerializeObject(union);
Console.WriteLine(jsonNet); // prints {"Case":"A","Fields":[5]}

var deserializedNet = JsonConvert.DeserializeObject<SimpleUnion>(json);
Console.WriteLine(union == deserializedNet); // prints true

// both serializers works in the same way:
Console.WriteLine(json == jsonNet); // prints true
Console.WriteLine(deserialized == deserializedNet); // prints true
```

If for some reason, you want to disable automatic converter registration, you can use `IgnoreJsonConverterAttribute`:

```c#
[Union(IgnoreJsonConverterAttribute = true)]
public partial record UnionWithIgnoredJsonAttribute
{
    static partial void Cases((int, string) a, string b);
}
```

```c#
using System.Text.Json;
using Newtonsoft.Json;

// (...)

// "{"Case":"A","Fields":[{"Item1":5,"Item2":"txt"}]}";
var json = "{\"Case\":\"A\",\"Fields\":[{\"Item1\":5,\"Item2\":\"txt\"}]}";

// System.Text.Json:
var options = new JsonSerializerOptions()
{
    Converters = {new UnionJsonConverterFactory()},
    IncludeFields = true // mandatory if tuple is serialized
};

JsonSerializer.Deserialize<UnionWithIgnoredJsonAttribute>(json, options); // works
JsonSerializer.Deserialize<UnionWithIgnoredJsonAttribute>(json); // throws an error

// Newtonsoft.Json:
var settings = new JsonSerializerSettings();
settings.Converters.Add(new UnionJsonNetConverter());

JsonConvert.DeserializeObject<UnionWithIgnoredJsonAttribute>(json, settings); // works
JsonConvert.DeserializeObject<UnionWithIgnoredJsonAttribute>(json); // throws an error
```

The serialization model is a composition of two values:
- `Case` of `string` for chosen case information,
- `Fields` array to keep the value of a case.

The model is compatible with Newtonosoft.Json's F# union converter and allows for deserializing F# union's JSON directly into generated union:

```f#
type FRecord = { X: int; Y: string }
type MyFsharpUnion =
    | A of int          // supports simple type
    | B of string       // supports string
    | F of FRecord      // supports F# record
    | T of string * int // DOES NOT SUPPORT F# tuple
```

```c#
[Union]
public partial record CSharpUnion
{
    static partial void Cases(int a, string b, FRecord f, (string, int) t);
}
```

```c#
using Newtonsoft.Json;

// (...)

var fUnion = MyFsharpUnion.NewA(5);
var json = JsonConvert.SerializeObject(fUnion);
Console.WriteLine(json); // prints {"Case":"A","Fields":[5]}

var output = JsonConvert.DeserializeObject<CSharpUnion>(json);
Console.WriteLine(output!.CastToA() == 5); // prints true
```

**NOTE:** F# tuple is **NOT** supported currently.

## License

Licensed under the [MIT License](LICENSE.txt).
