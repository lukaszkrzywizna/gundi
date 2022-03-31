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
        var json = UnionJsonConvert.SerializeObject(fUnion);
        var output = UnionJsonSerializer.Deserialize<CSharpUnion>(json);
        Assert.Equal(output!.CastToA(), a);
        output = UnionJsonConvert.DeserializeObject<CSharpUnion>(json);
        Assert.Equal(output!.CastToA(), a);
        
        // string
        const string b = "aaaaa";
        fUnion = MyFsharpUnion.NewB(b);
        json = UnionJsonConvert.SerializeObject(fUnion);
        output = UnionJsonSerializer.Deserialize<CSharpUnion>(json);
        Assert.Equal(output!.CastToB(), b);
        output = UnionJsonConvert.DeserializeObject<CSharpUnion>(json);
        Assert.Equal(output!.CastToB(), b);
        
        // F# record type
        fUnion = MyFsharpUnion.NewF(new FRecord(22, "yyyy"));
        json = UnionJsonConvert.SerializeObject(fUnion);
        output = UnionJsonSerializer.Deserialize<CSharpUnion>(json);
        Assert.Equal(output!.CastToF(), new FRecord(22, "yyyy"));
        output = UnionJsonConvert.DeserializeObject<CSharpUnion>(json);
        Assert.Equal(output!.CastToF(), new FRecord(22, "yyyy"));
        
        //F# tuple is not supported
        fUnion = MyFsharpUnion.NewT("xyz", 2);
        json = UnionJsonConvert.SerializeObject(fUnion);
        Assert.Throws<JsonException>(() => UnionJsonSerializer.Deserialize<CSharpUnion>(json));
        Assert.Throws<JsonSerializationException>(() => UnionJsonConvert.DeserializeObject<CSharpUnion>(json));
    }
    
    [Fact]
    public void UnionHasTuple_SerializersCanHandle()
    {
        var tuple = ("xyz", 22);
        var union = CSharpUnion.T(tuple);
        var json = UnionJsonSerializer.Serialize(union);
        Assert.Equal("{\"Case\":\"T\",\"Fields\":[{\"Item1\":\"xyz\",\"Item2\":22}]}", json);
        Assert.Equal(json, UnionJsonConvert.SerializeObject(union));
        var deserialized = UnionJsonSerializer.Deserialize<CSharpUnion>(json!);
        Assert.True(deserialized!.IsT());
        Assert.Equal(tuple, deserialized.CastToT());
        Assert.Equal(deserialized.CastToT(), UnionJsonConvert.DeserializeObject<CSharpUnion>(json!)!.CastToT());
    }
    
    [Fact]
    public void UnionWithGenericType_CanBeSerializedAndDeserializedBySystemJson()
    {
        var union = UnionWithJsonAttributes<State<decimal>>.Generic(new State<decimal>(5));
        var json = UnionJsonSerializer.Serialize(union);
        Assert.Equal("{\"Case\":\"Generic\",\"Fields\":[{\"Value\":5}]}", json);
        var deserialized = UnionJsonSerializer.Deserialize<UnionWithJsonAttributes<State<decimal>>>(json!);
        Assert.True(deserialized!.IsGeneric());
        Assert.Equal(new State<decimal>(5), deserialized.CastToGeneric());
    }
    
    [Fact]
    public void UnionWithGenericType_CanBeSerializedAndDeserializedByJsonNet()
    {
        var union = UnionWithJsonAttributes<State<decimal>>.Generic(new State<decimal>(5));
        var json = UnionJsonConvert.SerializeObject(union);
        Assert.Equal("{\"Case\":\"Generic\",\"Fields\":[{\"Value\":5.0}]}", json);
        var deserialized = UnionJsonConvert.DeserializeObject<UnionWithJsonAttributes<State<decimal>>>(json);
        Assert.True(deserialized!.IsGeneric());
        Assert.Equal(new State<decimal>(5), deserialized.CastToGeneric());
    }
}

[Union]
public partial record UnionWithJsonAttributes<T>
{
    static partial void Cases(int? a, string b, T generic);
}

[Union]
public partial record CSharpUnion
{
    static partial void Cases(int a, string b, FRecord f, (string, int) t);
}