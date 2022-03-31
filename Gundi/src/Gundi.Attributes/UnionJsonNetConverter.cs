using Newtonsoft.Json;

namespace Gundi;

public abstract class UnionJsonNetConverter<T> : JsonConverter<T>
{
    protected abstract T? NullValue { get; }
    protected abstract JsonUnion MapToJsonUnion(T value);
    protected abstract T UnionResolver(string caseName, Func<Type, object> deserialize);
    
    public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        void AssertMatchedProperty(string propertyName)
        {
            if (reader.TokenType != JsonToken.PropertyName || !string.Equals(reader.Value!.ToString(),
                    propertyName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException();
        }
        if (reader.TokenType == JsonToken.Null) return NullValue;
        reader.Read();
        AssertMatchedProperty(nameof(JsonUnion.Case));
        var caseName = reader.ReadAsString();
        reader.Read();
        AssertMatchedProperty(nameof(JsonUnion.Value));
        reader.Read();
        object Deserialize(Type t) => serializer.Deserialize(reader, t)!;
        var result = UnionResolver(caseName!, Deserialize);
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
        serializer.Serialize(writer, MapToJsonUnion(value));
    }
}