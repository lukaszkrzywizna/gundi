using System.Text.Json;
using System.Text.Json.Serialization;

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
        if(value!.Fields.Length != 1)
            throw new JsonException(
                "Union's fields can only contain an single object. F# union case tuple is not supported.");
        var caseValue = (JsonElement) value.Fields.Single();
        object Deserialize(Type t) => caseValue.Deserialize(t, options)!;
        return _jsonConverter.UnionResolver(value.Case, Deserialize);
    }
            
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, _jsonConverter.MapToJsonUnion(value), options);
    }
}

public class UnionJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => 
        typeToConvert.GetCustomAttributes(typeof(UnionAttribute), true).Any();

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var t = typeToConvert.GetNestedType("SystemConverter");

        if (typeToConvert.IsGenericType)
        {
            var g = t!.MakeGenericType(typeToConvert.GenericTypeArguments);
            return (JsonConverter) Activator.CreateInstance(g)!;
        }

        return (JsonConverter) Activator.CreateInstance(t!)!;
    }
}