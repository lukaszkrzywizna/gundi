#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Gundi;

namespace Gundi.Tests
{
    [System.Text.Json.Serialization.JsonConverter(typeof(UnionWithJsonAttributes<>.Converter))]
    [Newtonsoft.Json.JsonConverter(typeof(UnionWithJsonAttributes<>.NewtonsoftConverter))]
    partial record UnionWithJsonAttributes<T>
    {
        [Newtonsoft.Json.JsonPropertyAttribute]
        private readonly byte tag;
        
        [Newtonsoft.Json.JsonPropertyAttribute]
        private readonly System.Int32? a;
        [Newtonsoft.Json.JsonPropertyAttribute]
        private readonly System.String b;
        [Newtonsoft.Json.JsonPropertyAttribute]
        private readonly T generic;
        
        public bool IsA() => tag == 1;
        public bool IsB() => tag == 2;
        public bool IsGeneric() => tag == 3;

        [Newtonsoft.Json.JsonConstructorAttribute]
        private UnionWithJsonAttributes(
            System.Int32? a,
            System.String b,
            T generic,
            byte tag)
        {
            this.a = a;
            this.b = b;
            this.generic = generic;
            this.tag = tag;
        }
        
        public static UnionWithJsonAttributes<T> A(System.Int32? a) 
            => new(
                a,
                default!,
                default!,
                1
            );
        public static UnionWithJsonAttributes<T> B(System.String b) 
            => new(
                default!,
                b,
                default!,
                2
            );
        public static UnionWithJsonAttributes<T> Generic(T generic) 
            => new(
                default!,
                default!,
                generic,
                3
            );
        
        private string ActualCaseName()
        {
            return tag switch
            {
                1 => "A",
                2 => "B",
                3 => "Generic",
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public void Match(
            Action<System.Int32?> a,
            Action<System.String> b,
            Action<T> generic
            )
        {
            switch (tag)
            {
                case 1: 
                    a(this.a);
                    break;
                case 2: 
                    b(this.b);
                    break;
                case 3: 
                    generic(this.generic);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!");
            };
        }        
        
        public TOut Match<TOut>(
            Func<System.Int32?, TOut> a,
            Func<System.String, TOut> b,
            Func<T, TOut> generic
            )
        {
            return tag switch
            {
                1 => a(this.a),
                2 => b(this.b),
                3 => generic(this.generic),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public UnionWithJsonAttributes<T> Map(
            Func<System.Int32?, System.Int32?> a,
            Func<System.String, System.String> b,
            Func<T, T> generic
            )
        {
            return tag switch
            {
                1 => A(a(this.a)),
                2 => B(b(this.b)),
                3 => Generic(generic(this.generic)),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }        
        
        public override string ToString() => ActualCaseName();
        
        
        public TOut MatchA<TOut>(Func<System.Int32?, TOut> a, TOut _)
            => IsA() ? a(this.a) : _;
            
        public TOut MatchA<TOut>(Func<System.Int32?, TOut> a, Func<TOut> _)
            => IsA() ? a(this.a) : _();
                    
        public System.Int32? CastToA()
            => IsA() 
                ? this.a 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: A, Actual state: {ActualCaseName()}.");

        
        public TOut MatchB<TOut>(Func<System.String, TOut> b, TOut _)
            => IsB() ? b(this.b) : _;
            
        public TOut MatchB<TOut>(Func<System.String, TOut> b, Func<TOut> _)
            => IsB() ? b(this.b) : _();
                    
        public System.String CastToB()
            => IsB() 
                ? this.b 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: B, Actual state: {ActualCaseName()}.");

        
        public TOut MatchGeneric<TOut>(Func<T, TOut> generic, TOut _)
            => IsGeneric() ? generic(this.generic) : _;
            
        public TOut MatchGeneric<TOut>(Func<T, TOut> generic, Func<TOut> _)
            => IsGeneric() ? generic(this.generic) : _();
                    
        public T CastToGeneric()
            => IsGeneric() 
                ? this.generic 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: Generic, Actual state: {ActualCaseName()}.");

        
        private class Converter : System.Text.Json.Serialization.JsonConverter<UnionWithJsonAttributes<T>>
        {
            public override UnionWithJsonAttributes<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null) return null;
                var value = JsonSerializer.Deserialize<JsonUnion>(ref reader, options);
                var caseValue = (JsonElement) value!.Value;
                return value.Case switch
                {
                    "A" => A(caseValue.Deserialize<System.Int32?>(options)!),
                    "B" => B(caseValue.Deserialize<System.String>(options)!),
                    "Generic" => Generic(caseValue.Deserialize<T>(options)!),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), value.Case, "Union has undefined state!")
                };
            }
        
            public override void Write(Utf8JsonWriter writer, UnionWithJsonAttributes<T> value, JsonSerializerOptions options)
            {
                var obj = value.Match(
                    a => a as object,
                    b => b as object,
                    generic => generic as object
                    );
                JsonSerializer.Serialize(writer, new JsonUnion(value.ActualCaseName(), obj!));
            }
        }
        
        private class NewtonsoftConverter : Newtonsoft.Json.JsonConverter<UnionWithJsonAttributes<T>>
        {
            public override UnionWithJsonAttributes<T>? ReadJson(JsonReader reader, Type objectType, UnionWithJsonAttributes<T>? existingValue, bool hasExistingValue,
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
                TCase Deserialize<TCase>() => serializer.Deserialize<TCase>(reader)!;
                var result = caseName switch
                {
                    "A" => A(Deserialize<System.Int32?>()),
                    "B" => B(Deserialize<System.String>()),
                    "Generic" => Generic(Deserialize<T>()),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), caseName, "Union has undefined state!")
                };
                reader.Read();
                return result;
            }
            public override void WriteJson(JsonWriter writer, UnionWithJsonAttributes<T>? value, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (value is null)
                { 
                    writer.WriteNull();
                    return;
                }
                var obj = value.Match(
                    a => a as object,
                    b => b as object,
                    generic => generic as object
                    );
                serializer.Serialize(writer, new JsonUnion(value!.ActualCaseName(), obj!));
            }
        }
    }
}