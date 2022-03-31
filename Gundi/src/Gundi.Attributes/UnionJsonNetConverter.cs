using Newtonsoft.Json;

namespace Gundi;

public class UnionJsonNetConverter<T> : Newtonsoft.Json.JsonConverter<T>
{
    private readonly JsonConverter<T> _jsonConverter;

    public UnionJsonNetConverter(JsonConverter<T> jsonConverter)
    {
        _jsonConverter = jsonConverter;
    }
    
    public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        void AssertMatchedProperty(string propertyName)
        {
            if (reader.TokenType != JsonToken.PropertyName || !string.Equals(reader.Value!.ToString(),
                    propertyName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException();
        }
        if (reader.TokenType == JsonToken.Null) return _jsonConverter.NullValue;
        reader.Read();
        AssertMatchedProperty(nameof(JsonUnion.Case));
        var caseName = reader.ReadAsString();
        reader.Read();
        AssertMatchedProperty(nameof(JsonUnion.Value));
        reader.Read();
        object Deserialize(Type t) => serializer.Deserialize(reader, t)!;
        var result = _jsonConverter.UnionResolver(caseName!, Deserialize);
        reader.Read();
        return result;
    }

    public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
    {
        if (value is null)
        { 
            writer.WriteNull();
            return;
        }
        serializer.Serialize(writer, _jsonConverter.MapToJsonUnion(value));
    }
}