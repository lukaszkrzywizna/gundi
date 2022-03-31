using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Gundi.Tests;

public static class UnionJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters =
        {
            new UnionJsonConverterFactory()
        },
        IncludeFields = true
    };

    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);
    public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
}

public static class UnionJsonConvert
{
    static UnionJsonConvert()
    {
        Options = new JsonSerializerSettings();
        Options.Converters.Add(new UnionJsonNetConverter());
    }
    
    private static readonly JsonSerializerSettings Options;

    public static string SerializeObject<T>(T value) => JsonConvert.SerializeObject(value, Options);
    public static T? DeserializeObject<T>(string json) => JsonConvert.DeserializeObject<T>(json, Options);
}