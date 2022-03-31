#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Gundi;

namespace Gundi.Tests
{
    [System.Text.Json.Serialization.JsonConverter(typeof(UnionWithCustomException.Converter))]
    [Newtonsoft.Json.JsonConverter(typeof(UnionWithCustomException.NewtonsoftConverter))]
    partial record UnionWithCustomException
    {
        
        private readonly byte tag;
        
        
        private readonly System.Int32 a;
        
        private readonly System.String b;
        
        public bool IsA() => tag == 1;
        public bool IsB() => tag == 2;

        private UnionWithCustomException(
            System.Int32 a,
            System.String b,
            byte tag)
        {
            this.a = a;
            this.b = b;
            this.tag = tag;
        }
        
        public static UnionWithCustomException A(System.Int32 a) 
            => new(
                a,
                default!,
                1
            );
        public static UnionWithCustomException B(System.String b) 
            => new(
                default!,
                b,
                2
            );
        
        private string ActualCaseName()
        {
            return tag switch
            {
                1 => "A",
                2 => "B",
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public void Match(
            Action<System.Int32> a,
            Action<System.String> b
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
                default: throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!");
            };
        }        
        
        public TOut Match<TOut>(
            Func<System.Int32, TOut> a,
            Func<System.String, TOut> b
            )
        {
            return tag switch
            {
                1 => a(this.a),
                2 => b(this.b),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public UnionWithCustomException Map(
            Func<System.Int32, System.Int32> a,
            Func<System.String, System.String> b
            )
        {
            return tag switch
            {
                1 => A(a(this.a)),
                2 => B(b(this.b)),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }        
        
        public override string ToString() => ActualCaseName();
        
        
        public TOut MatchA<TOut>(Func<System.Int32, TOut> a, TOut _)
            => IsA() ? a(this.a) : _;
            
        public TOut MatchA<TOut>(Func<System.Int32, TOut> a, Func<TOut> _)
            => IsA() ? a(this.a) : _();
                    
        public System.Int32 CastToA()
            => IsA() 
                ? this.a 
                : throw new AnotherAssembly.MyException(this.GetType(), nameof(A), ActualCaseName());
        
        public TOut MatchB<TOut>(Func<System.String, TOut> b, TOut _)
            => IsB() ? b(this.b) : _;
            
        public TOut MatchB<TOut>(Func<System.String, TOut> b, Func<TOut> _)
            => IsB() ? b(this.b) : _();
                    
        public System.String CastToB()
            => IsB() 
                ? this.b 
                : throw new AnotherAssembly.MyException(this.GetType(), nameof(B), ActualCaseName());
        
        private class Converter : System.Text.Json.Serialization.JsonConverter<UnionWithCustomException>
        {
            public override UnionWithCustomException? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null) return null;
                var value = JsonSerializer.Deserialize<JsonUnion>(ref reader, options);
                var caseValue = (JsonElement) value!.Value;
                return value.Case switch
                {
                    "A" => A(caseValue.Deserialize<System.Int32>(options)!),
                    "B" => B(caseValue.Deserialize<System.String>(options)!),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), value.Case, "Union has undefined state!")
                };
            }
        
            public override void Write(Utf8JsonWriter writer, UnionWithCustomException value, JsonSerializerOptions options)
            {
                var obj = value.Match(
                    a => a as object,
                    b => b as object
                    );
                JsonSerializer.Serialize(writer, new JsonUnion(value.ActualCaseName(), obj!));
            }
        }
        
        private class NewtonsoftConverter : Newtonsoft.Json.JsonConverter<UnionWithCustomException>
        {
            public override UnionWithCustomException? ReadJson(JsonReader reader, Type objectType, UnionWithCustomException? existingValue, bool hasExistingValue,
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
                    "A" => A(Deserialize<System.Int32>()),
                    "B" => B(Deserialize<System.String>()),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), caseName, "Union has undefined state!")
                };
                reader.Read();
                return result;
            }
            public override void WriteJson(JsonWriter writer, UnionWithCustomException? value, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (value is null)
                { 
                    writer.WriteNull();
                    return;
                }
                var obj = value.Match(
                    a => a as object,
                    b => b as object
                    );
                serializer.Serialize(writer, new JsonUnion(value!.ActualCaseName(), obj!));
            }
        }
    }
}