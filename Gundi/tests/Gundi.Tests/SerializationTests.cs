using Gundi.Tests.Internal;
using Newtonsoft.Json;
using TestAssembly;
using Xunit;
using JsonException = System.Text.Json.JsonException;

namespace Gundi.Tests;

public class SerializationTests
{
    [Fact]
    public void FSharpUnion_SerializersCanHandle()
    {
        // simple type
        const int a = 5;
        var fUnion = MyFsharpUnion.NewA(a);
        var json = SimpleJsonConvert.SerializeObject(fUnion);
        var output = SimpleJsonSerializer.Deserialize<CSharpUnion>(json);
        Assert.Equal(output!.CastToA(), a);
        output = SimpleJsonConvert.DeserializeObject<CSharpUnion>(json);
        Assert.Equal(output!.CastToA(), a);
        
        // string
        const string b = "aaaaa";
        fUnion = MyFsharpUnion.NewB(b);
        json = SimpleJsonConvert.SerializeObject(fUnion);
        output = SimpleJsonSerializer.Deserialize<CSharpUnion>(json);
        Assert.Equal(output!.CastToB(), b);
        output = SimpleJsonConvert.DeserializeObject<CSharpUnion>(json);
        Assert.Equal(output!.CastToB(), b);
        
        // F# record type
        fUnion = MyFsharpUnion.NewF(new FRecord(22, "yyyy"));
        json = SimpleJsonConvert.SerializeObject(fUnion);
        output = SimpleJsonSerializer.Deserialize<CSharpUnion>(json);
        Assert.Equal(output!.CastToF(), new FRecord(22, "yyyy"));
        output = SimpleJsonConvert.DeserializeObject<CSharpUnion>(json);
        Assert.Equal(output!.CastToF(), new FRecord(22, "yyyy"));
        
        //F# tuple is not supported
        fUnion = MyFsharpUnion.NewT("xyz", 2);
        json = SimpleJsonConvert.SerializeObject(fUnion);
        Assert.Throws<JsonException>(() => SimpleJsonSerializer.Deserialize<CSharpUnion>(json));
        Assert.Throws<JsonSerializationException>(() => SimpleJsonConvert.DeserializeObject<CSharpUnion>(json));
    }
    
    [Fact]
    public void UnionHasTuple_SerializersCanHandle()
    {
        var tuple = ("xyz", 22);
        var union = CSharpUnion.T(tuple);
        var json = SimpleJsonSerializer.Serialize(union);
        Assert.Equal("{\"Case\":\"T\",\"Fields\":[{\"Item1\":\"xyz\",\"Item2\":22}]}", json);
        Assert.Equal(json, SimpleJsonConvert.SerializeObject(union));
        var deserialized = SimpleJsonSerializer.Deserialize<CSharpUnion>(json!);
        Assert.True(deserialized!.IsT());
        Assert.Equal(tuple, deserialized.CastToT());
        Assert.Equal(deserialized.CastToT(), SimpleJsonConvert.DeserializeObject<CSharpUnion>(json!)!.CastToT());
    }
    
    [Fact]
    public void UnionWithGenericType_CanBeSerializedAndDeserializedBySystemJson()
    {
        var union = UnionWithGeneric<State<decimal>>.Generic(new State<decimal>(5));
        var json = SimpleJsonSerializer.Serialize(union);
        Assert.Equal("{\"Case\":\"Generic\",\"Fields\":[{\"Value\":5}]}", json);
        var deserialized = SimpleJsonSerializer.Deserialize<UnionWithGeneric<State<decimal>>>(json!);
        Assert.True(deserialized!.IsGeneric());
        Assert.Equal(new State<decimal>(5), deserialized.CastToGeneric());
    }
    
    [Fact]
    public void UnionWithGenericType_CanBeSerializedAndDeserializedByJsonNet()
    {
        var union = UnionWithGeneric<State<decimal>>.Generic(new State<decimal>(5));
        var json = SimpleJsonConvert.SerializeObject(union);
        Assert.Equal("{\"Case\":\"Generic\",\"Fields\":[{\"Value\":5.0}]}", json);
        var deserialized = SimpleJsonConvert.DeserializeObject<UnionWithGeneric<State<decimal>>>(json);
        Assert.True(deserialized!.IsGeneric());
        Assert.Equal(new State<decimal>(5), deserialized.CastToGeneric());
    }
    
    [Fact]
    public void UnionUseIgnoreJsonAttributeSetting_CanBeSerializedOnlyWithManualAddedSystemJsonConverter()
    {
        var union = UnionWithIgnoredJsonAttribute.A((5, "txt"));
        var json = SimpleJsonSerializer.Serialize(union);
        Assert.Equal("{}", json);
        Assert.Throws<NotSupportedException>(() =>
            SimpleJsonSerializer.Deserialize<UnionWithIgnoredJsonAttribute>(json));
        
        json = UnionJsonSerializer.Serialize(union);
        Assert.Equal("{\"Case\":\"A\",\"Fields\":[{\"Item1\":5,\"Item2\":\"txt\"}]}", json);
        var deserialized = UnionJsonSerializer.Deserialize<UnionWithIgnoredJsonAttribute>(json);

        Assert.True(deserialized!.IsA());
        Assert.Equal((5, "txt"), deserialized.CastToA());
    }
    
    [Fact]
    public void UnionUseIgnoreJsonAttributeSetting_CanBeSerializedOnlyWithManualAddedJsonNetConverter()
    {
        var union = UnionWithIgnoredJsonAttribute.A((5, "txt"));
        var json = SimpleJsonConvert.SerializeObject(union);
        Assert.Equal("{}", json);
        Assert.Throws<JsonSerializationException>(() =>
            SimpleJsonConvert.DeserializeObject<UnionWithIgnoredJsonAttribute>(json));
        
        json = UnionJsonConvert.SerializeObject(union);
        Assert.Equal("{\"Case\":\"A\",\"Fields\":[{\"Item1\":5,\"Item2\":\"txt\"}]}", json);
        var deserialized = UnionJsonConvert.DeserializeObject<UnionWithIgnoredJsonAttribute>(json);

        Assert.True(deserialized!.IsA());
        Assert.Equal((5, "txt"), deserialized.CastToA());
    }
}

[Union]
public partial record UnionWithGeneric<T>
{
    static partial void Cases(int? a, string b, T generic);
}

[Union(IgnoreJsonConverterAttribute = true)]
public partial record UnionWithIgnoredJsonAttribute
{
    static partial void Cases((int, string) a, string b);
}

[Union]
public partial record CSharpUnion
{
    static partial void Cases(int a, string b, FRecord f, (string, int) t);
}