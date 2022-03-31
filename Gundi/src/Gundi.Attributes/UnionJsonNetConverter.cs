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
        AssertMatchedProperty(nameof(JsonUnion.Fields));
        reader.Read();
        if (reader.TokenType != JsonToken.StartArray)
            throw new JsonSerializationException("Missing an array for a union's fields");
        reader.Read();
        object Deserialize(Type t) => serializer.Deserialize(reader, t)!;
        var result = _jsonConverter.UnionResolver(caseName!, Deserialize);
        reader.Read();
        if (reader.TokenType != JsonToken.EndArray)
            throw new JsonSerializationException(
                "Union's fields can only contain an single object. F# union case tuple is not supported.");
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

public class UnionJsonNetConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }
        var converter = CreateConverter(value.GetType());
        converter.WriteJson(writer, value, serializer);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var converter = CreateConverter(objectType);
        return converter.ReadJson(reader, objectType, existingValue, serializer);
    }

    public override bool CanConvert(Type objectType)=> 
        objectType.GetCustomAttributes(typeof(UnionAttribute), true).Any();
    
    private static JsonConverter CreateConverter(Type typeToConvert)
    {
        var t = typeToConvert.GetNestedType("JsonNetConverter");

        if (typeToConvert.IsGenericType)
        {
            var g = t!.MakeGenericType(typeToConvert.GenericTypeArguments);
            return (JsonConverter) Activator.CreateInstance(g)!;
        }

        return (JsonConverter) Activator.CreateInstance(t!)!;
    }
}