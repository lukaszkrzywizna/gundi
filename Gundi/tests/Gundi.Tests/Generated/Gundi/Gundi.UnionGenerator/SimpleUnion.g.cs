#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gundi.Tests
{
    [System.Text.Json.Serialization.JsonConverter(typeof(SimpleUnion.Converter))]
    partial record SimpleUnion
    {
        [Newtonsoft.Json.JsonPropertyAttribute]
        private readonly byte tag;
        
        [Newtonsoft.Json.JsonPropertyAttribute]
        private readonly System.Int32 a;
        [Newtonsoft.Json.JsonPropertyAttribute]
        private readonly System.String b;
        [Newtonsoft.Json.JsonPropertyAttribute]
        private readonly System.Decimal c;
        [Newtonsoft.Json.JsonPropertyAttribute]
        private readonly System.Int32? d;
        
        public bool IsA() => tag == 1;
        public bool IsB() => tag == 2;
        public bool IsC() => tag == 3;
        public bool IsD() => tag == 4;

        [Newtonsoft.Json.JsonConstructorAttribute]
        private SimpleUnion(
            System.Int32 a,
            System.String b,
            System.Decimal c,
            System.Int32? d,
            byte tag)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.tag = tag;
        }
        
        public static SimpleUnion A(System.Int32 a) 
            => new(
                a,
                default!,
                default!,
                default!,
                1
            );
        public static SimpleUnion B(System.String b) 
            => new(
                default!,
                b,
                default!,
                default!,
                2
            );
        public static SimpleUnion C(System.Decimal c) 
            => new(
                default!,
                default!,
                c,
                default!,
                3
            );
        public static SimpleUnion D(System.Int32? d) 
            => new(
                default!,
                default!,
                default!,
                d,
                4
            );
        
        private string ActualCaseName()
        {
            return tag switch
            {
                1 => "A",
                2 => "B",
                3 => "C",
                4 => "D",
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public void Match(
            Action<System.Int32> a,
            Action<System.String> b,
            Action<System.Decimal> c,
            Action<System.Int32?> d
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
                    c(this.c);
                    break;
                case 4: 
                    d(this.d);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!");
            };
        }        
        
        public TOut Match<TOut>(
            Func<System.Int32, TOut> a,
            Func<System.String, TOut> b,
            Func<System.Decimal, TOut> c,
            Func<System.Int32?, TOut> d
            )
        {
            return tag switch
            {
                1 => a(this.a),
                2 => b(this.b),
                3 => c(this.c),
                4 => d(this.d),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public SimpleUnion Map(
            Func<System.Int32, System.Int32> a,
            Func<System.String, System.String> b,
            Func<System.Decimal, System.Decimal> c,
            Func<System.Int32?, System.Int32?> d
            )
        {
            return tag switch
            {
                1 => A(a(this.a)),
                2 => B(b(this.b)),
                3 => C(c(this.c)),
                4 => D(d(this.d)),
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

        
        public TOut MatchC<TOut>(Func<System.Decimal, TOut> c, TOut _)
            => IsC() ? c(this.c) : _;
            
        public TOut MatchC<TOut>(Func<System.Decimal, TOut> c, Func<TOut> _)
            => IsC() ? c(this.c) : _();
                    
        public System.Decimal CastToC()
            => IsC() 
                ? this.c 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: C, Actual state: {ActualCaseName()}.");

        
        public TOut MatchD<TOut>(Func<System.Int32?, TOut> d, TOut _)
            => IsD() ? d(this.d) : _;
            
        public TOut MatchD<TOut>(Func<System.Int32?, TOut> d, Func<TOut> _)
            => IsD() ? d(this.d) : _();
                    
        public System.Int32? CastToD()
            => IsD() 
                ? this.d 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: D, Actual state: {ActualCaseName()}.");

        
        private class Converter : System.Text.Json.Serialization.JsonConverter<SimpleUnion>
        {
            private record JsonUnion(string Case, object Value);
            
            public override SimpleUnion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null) return null;
                var value = JsonSerializer.Deserialize<JsonUnion>(ref reader, options);
                var caseValue = (JsonElement) value!.Value;
                return value.Case switch
                {
                    "A" => A(caseValue.Deserialize<System.Int32>(options)!),
                    "B" => B(caseValue.Deserialize<System.String>(options)!),
                    "C" => C(caseValue.Deserialize<System.Decimal>(options)!),
                    "D" => D(caseValue.Deserialize<System.Int32?>(options)!),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), value.Case, "Union has undefined state!")
                };
            }
        
            public override void Write(Utf8JsonWriter writer, SimpleUnion value, JsonSerializerOptions options)
            {
                var obj = value.Match(
                    a => a as object,
                    b => b as object,
                    c => c as object,
                    d => d as object
                    );
                JsonSerializer.Serialize(writer, new JsonUnion(value.ActualCaseName(), obj!));
            }
        }
    }
}