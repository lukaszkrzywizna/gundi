using System.Globalization;
using System.Text.Json;
using AnotherAssembly;
using Gundi.Tests.Internal;
using Newtonsoft.Json;
using Xunit;
using JsonConstructorAttribute = Newtonsoft.Json.JsonConstructorAttribute;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Gundi.Tests;

[Serializable]
public abstract record MyOption
{
    internal MyOption(){}
    internal record SomeCase(int Item) : MyOption;
    internal record NoneCase : MyOption;

    public static MyOption Some(int value) => new SomeCase(value);
    public static MyOption None = new NoneCase();

    public bool IsSome => this is SomeCase;
    public bool IsNone => this is NoneCase;

    public TOut Match<TOut>(Func<int, TOut> some, Func<TOut> none)
    {
        return this switch
        {
            SomeCase s => some(s.Item),
            NoneCase => none(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

internal record JsonUnion(string Case, object Value);


[System.Text.Json.Serialization.JsonConverter(typeof(UnionConverter))]
[Newtonsoft.Json.JsonConverter(typeof(UnionJConverter))]
public record Union
{
    private readonly byte tag;
    private readonly int a;
    private readonly Testa b;

    private Union(byte tag, int a, Testa b)
    {
        this.tag = tag;
        this.a = a;
        this.b = b;
    }

    public static Union A(int a) => new Union(1, a, default!);
    public static Union B(Testa b) => new Union(2, default!, b);

    public TOut Map<TOut>(Func<int, TOut> a, Func<Testa, TOut> b)
    {
        return tag switch
        {
            1 => a(this.a),
            2 => b(this.b),
            _ => throw new InvalidOperationException("undefined union")
        };
    }
    
    public void Map(Action<int> a, Action<Testa> b)
    {
        switch (tag)
        {
            case 1:
                a(this.a);
                break;
            case 2:
                b(this.b);
                break;
            default: throw new InvalidOperationException("undefined union");
        };
    }

    private string ActualCaseName() =>
        tag switch
        {
            1 => "A",
            2 => "B",
            _ => throw new Exception("dd")
        };
    
    private class UnionConverter : System.Text.Json.Serialization.JsonConverter<Union>
    {
        public override Union? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            var value = JsonSerializer.Deserialize<JsonUnion>(ref reader, options);
            var caseValue = (JsonElement) value!.Value;
            return value.Case switch
            {
                "A" => A(caseValue.Deserialize<int>(options)!),
                "B" => B(caseValue.Deserialize<Testa>(options)!),
                _ => throw new InvalidOperationException()
            };
        }

        public override void Write(Utf8JsonWriter writer, Union value, JsonSerializerOptions options)
        {
            var obj = value.Map(a => a as object, b => b);
            JsonSerializer.Serialize(writer, new JsonUnion(value.ActualCaseName(), obj));
        }
    }

    private class UnionJConverter : Newtonsoft.Json.JsonConverter<Union>
    {
        public override Union? ReadJson(JsonReader reader, Type objectType, Union? existingValue, bool hasExistingValue,
            Newtonsoft.Json.JsonSerializer serializer)
        {
            void AssertMatchedProperty(string propertyName)
            {
                if (reader.TokenType != JsonToken.PropertyName || !string.Equals(reader.Value!.ToString(),
                        propertyName, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException();
            }

            if (reader.TokenType == JsonToken.Null) return null;
            reader.Read();
            AssertMatchedProperty(nameof(JsonUnion.Case));
            var caseName = reader.ReadAsString();
            reader.Read();
            AssertMatchedProperty(nameof(JsonUnion.Value));
            reader.Read();
            var result = caseName switch
            {
                "A" => A(serializer.Deserialize<int>(reader)!),
                "B" => B(serializer.Deserialize<Testa>(reader)!),
                _ => throw new InvalidOperationException()
            };
            reader.Read();
            return result;
        }

        public override void WriteJson(JsonWriter writer, Union? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value is null) writer.WriteNull();
            serializer.Serialize(writer, new JsonUnion(value!.ActualCaseName(), value.Map(a => a as object, b => b)));
        }
    }
}

public record Testa(int A);

public class Tests
{
    [Fact]
    public void UnionWithSimpleTypes_FactoryAndCheckMethodsGenerated()
    {
        var testa = new Testa(5);
        var tt = JsonConvert.SerializeObject(testa);
        var ttt = JsonSerializer.Serialize(testa);
        
        var uu = Union.B(testa);
        var jc = JsonConvert.SerializeObject(uu);
        var jcc = JsonSerializer.Serialize(uu);
        JsonSerializer.Deserialize<Union>(jcc);
        var deser = JsonSerializer.Deserialize<Union>(jc);
        var j = JsonSerializer.Serialize(deser);
        var jcDeser = JsonConvert.DeserializeObject<Union>(j);

        var a = SimpleUnion.A(5);
        Assert.True(a.IsA());
        Assert.False(a.IsB());
        Assert.False(a.IsC());
        Assert.False(a.IsD());
        var b = SimpleUnion.B("5");
        Assert.True(b.IsB());
        var c = SimpleUnion.C(5);
        Assert.True(c.IsC());
        var d = SimpleUnion.D(null);
        Assert.True(d.IsD());
    }

    [Fact]
    public void UnionWithSimpleTypes_MatchMethodsGenerated()
    {
        var union = SimpleUnion.A(5);
        var mapped = union.Match(
            a => a.ToString(),
            b => b,
            c => c.ToString(CultureInfo.InvariantCulture),
            d => d?.ToString());

        Assert.Equal("5", mapped);
        Assert.Equal("5", union.MatchA(x => x.ToString(), "default"));
        Assert.Equal("default", union.MatchB(x => x, "default"));
        Assert.Equal("default", union.MatchC(x => x.ToString(CultureInfo.InvariantCulture), "default"));
        Assert.Equal("default", union.MatchD(x => x.ToString(), "default"));
    }
    
    [Fact]
    public void UnionWithSimpleTypes_MapMethodGenerated()
    {
        var union = SimpleUnion.A(5);
        var mapped = union.Map(
            a => a,
            b => b,
            c => c,
            d => d);

        Assert.Equal(union, mapped);
    }
    
    [Fact]
    public void UnionWithCustomException_DuringCastThrowsCustomException()
    {
        var unionA = UnionWithCustomException.A(5);
        var exA = Assert.Throws<MyException>(() => unionA.CastToB());
        Assert.Contains(nameof(UnionWithCustomException), exA.Message);
        Assert.Contains($"Expected: {nameof(UnionWithCustomException.B)}", exA.Message);
        Assert.Contains($"Actual: {nameof(UnionWithCustomException.A)}", exA.Message);
        
        var unionB = UnionWithCustomException.B("5");
        var exB = Assert.Throws<MyException>(() => unionB.CastToA());
        Assert.Contains($"Expected: {nameof(UnionWithCustomException.A)}", exB.Message);
        Assert.Contains($"Actual: {nameof(UnionWithCustomException.B)}", exB.Message);
    }
    
    [Fact]
    public void UnionWithJsonAttributesAndGenericType_UnionCanBeSerializedAndDeserialized()
    {
        var union = UnionWithJsonAttributes<State<decimal>>.Generic(new State<decimal>(5));
        var json = JsonConvert.SerializeObject(union);
        Assert.Equal("{\"tag\":3,\"a\":null,\"b\":null,\"generic\":{\"Value\":5.0}}", json);
        var deserialized = JsonConvert.DeserializeObject<UnionWithJsonAttributes<State<decimal>?>>(json);
        Assert.True(deserialized!.IsGeneric());
        Assert.Equal(new State<decimal>(5), deserialized.CastToGeneric());
    }
}

[Union(FieldAttribute = typeof(JsonPropertyAttribute), ConstructorAttribute = typeof(JsonConstructorAttribute))]
public partial record SimpleUnion
{
    static partial void Cases(int a, string b, decimal c, int? d);
}

[Union(CustomCastException = typeof(MyException))]
public partial record UnionWithCustomException
{
    static partial void Cases(int a, string b);
}

[Union(FieldAttribute = typeof(JsonPropertyAttribute), ConstructorAttribute = typeof(JsonConstructorAttribute))]
public partial record UnionWithJsonAttributes<T>
{
    static partial void Cases(int? a, string b, T generic);
}