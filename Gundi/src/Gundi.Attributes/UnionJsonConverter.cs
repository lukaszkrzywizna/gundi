using System.Text.Json;

namespace Gundi;

public class Converter<T>
{
    
}

public abstract class UnionJsonConverter<T> : System.Text.Json.Serialization.JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return NullValue;
        var value = JsonSerializer.Deserialize<JsonUnion>(ref reader, options);
        var caseValue = (JsonElement) value!.Value;
        object Deserialize(Type t) => caseValue.Deserialize(t, options)!;
        return UnionResolver(value.Case, Deserialize);
    }
            
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, MapToJsonUnion(value));
    }

    protected abstract JsonUnion MapToJsonUnion(T value);
    protected abstract T UnionResolver(string caseName, Func<Type, object> deserialize);
    protected abstract T? NullValue { get; }
}