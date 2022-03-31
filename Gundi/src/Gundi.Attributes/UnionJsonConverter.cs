using System.Text.Json;

namespace Gundi;

public class UnionJsonConverter<T> : System.Text.Json.Serialization.JsonConverter<T>
{
    private readonly JsonConverter<T> _jsonConverter;

    public UnionJsonConverter(JsonConverter<T> jsonConverter)
    {
        _jsonConverter = jsonConverter;
    }
    
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return _jsonConverter.NullValue;
        var value = JsonSerializer.Deserialize<JsonUnion>(ref reader, options);
        var caseValue = (JsonElement) value!.Value.Single();
        object Deserialize(Type t) => caseValue.Deserialize(t, options)!;
        return _jsonConverter.UnionResolver(value.Case, Deserialize);
    }
            
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, _jsonConverter.MapToJsonUnion(value));
    }
}