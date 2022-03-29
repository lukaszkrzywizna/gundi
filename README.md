# gundi
A generator for discriminated union for C# (based on union in F#)

- [Overview](#overview)
  - [Defining a union](#defining-a-union)
  - [Using](#using)
  - [Union casting](#union-casting)
  - [Serialization](#serialization)
- [License](#license)

## Overview
A union is a value that represents several different cases of a different name or/and type. It is useful for modeling more complex choice types - to express that provided data could be shaped differently. For example, Union `TxtOrNumber` of a `string txt` and `int number` informs, that type can contain a `string` or `int`, but never both.

### Defining a union
In order to use a generator, there has to be a defined simple union schema:
```csharp
using Gundi;

namespace MyNamespace

[Union] // Mandatory attribute
public partial record SimpleUnion
{
    static partial void Cases(int a, string b, decimal c, int? d); // Mandatory function. `static partial void Cases` is a must-have
}
```

The `Union` attribute applies for a partial record (struct and class is NOT ALLOWED). It helps the generator, to identify which types should be enhanced.
The `Cases` method uses specified types and argument names to define union cases.

The generator will generate a partial record which will contain all arguments kept as a private field and some public API:
```csharp

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
```csharp
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

The generator will generate `Cast` functions, which "force" to get a defined union case or throws an exception. By default, `InvalidOperationException` is thrown, but there is a possibility to override the type with `CustomCastException` setting:
```csharp
[Union(CustomCastException = typeof(MyException))]
public partial record UnionWithCustomException
{
    static partial void Cases(int a, string b);
}
```
The selected type must be an exception with a constructor with three arguments

```csharp
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

Due to generating fields and constructor with `private` modifier, the record can't be deserialized. As a workaround, `Union` attribute provides the ability to define a custom field and constructor attribute:

```csharp
using Newtonsoft.Json; 
namespace MyNamespace;

[Union(FieldAttribute = typeof(JsonPropertyAttribute), ConstructorAttribute = typeof(JsonConstructorAttribute))]
public partial record UnionWithJsonAttributes<T>
{
    static partial void Cases(int? a, string b, T generic);
}
```

That defined union can be easily serialized:

```csharp
var union = UnionwithJsonAttributes<int>.Generic(5);
var json = JsonSerializer.SerializeObject(union);
Console.WriteLine(json); // prints {"tag":3,"a":null,"b":null,"generic":5}
var deserialized = JsonSerializer.DeserializeObject<UnionwithJsonAttributes<int>>(json);
Console.WriteLine(union == deserialized); // prints true
```

`JsonPropertyAttribute` and `JsonConstructorAttribute` are attributes provided by `Newtonsoft.Json`. Currently, there is no possibility to use Microsoft's `System.Text.Json` serializer.

## License
Licensed under the [MIT License](LICENSE.txt).
