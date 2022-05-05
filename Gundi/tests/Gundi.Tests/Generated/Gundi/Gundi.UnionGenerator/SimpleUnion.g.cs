#nullable enable 

using System;
using Gundi;

namespace Gundi.Tests
{
    [System.Text.Json.Serialization.JsonConverter(typeof(UnionJsonConverterFactory))]
    [Newtonsoft.Json.JsonConverter(typeof(UnionJsonNetConverter))]
    partial record SimpleUnion
    {
        private readonly byte tag;
        
        private readonly System.Int32 a;
        private readonly System.String b;
        private readonly System.Decimal c;
        private readonly System.Int32? d;
        private readonly Gundi.Tests.Record? nullableRecord;
        
        public bool IsA() => tag == 1;
        public bool IsB() => tag == 2;
        public bool IsC() => tag == 3;
        public bool IsD() => tag == 4;
        public bool IsNullableRecord() => tag == 5;

        private SimpleUnion(
            System.Int32 a,
            System.String b,
            System.Decimal c,
            System.Int32? d,
            Gundi.Tests.Record? nullableRecord,
            byte tag)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.nullableRecord = nullableRecord;
            this.tag = tag;
        }
        
        public static SimpleUnion A(System.Int32 a) 
            => new(
                a,
                default!,
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
                default!,
                2
            );
        public static SimpleUnion C(System.Decimal c) 
            => new(
                default!,
                default!,
                c,
                default!,
                default!,
                3
            );
        public static SimpleUnion D(System.Int32? d) 
            => new(
                default!,
                default!,
                default!,
                d,
                default!,
                4
            );
        public static SimpleUnion NullableRecord(Gundi.Tests.Record? nullableRecord) 
            => new(
                default!,
                default!,
                default!,
                default!,
                nullableRecord,
                5
            );
        
        private string ActualCaseName()
        {
            return tag switch
            {
                1 => "A",
                2 => "B",
                3 => "C",
                4 => "D",
                5 => "NullableRecord",
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public void Match(
            Action<System.Int32> a,
            Action<System.String> b,
            Action<System.Decimal> c,
            Action<System.Int32?> d,
            Action<Gundi.Tests.Record?> nullableRecord
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
                case 5: 
                    nullableRecord(this.nullableRecord);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!");
            };
        }        
        
        public TOut Match<TOut>(
            Func<System.Int32, TOut> a,
            Func<System.String, TOut> b,
            Func<System.Decimal, TOut> c,
            Func<System.Int32?, TOut> d,
            Func<Gundi.Tests.Record?, TOut> nullableRecord
            )
        {
            return tag switch
            {
                1 => a(this.a),
                2 => b(this.b),
                3 => c(this.c),
                4 => d(this.d),
                5 => nullableRecord(this.nullableRecord),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public SimpleUnion Map(
            Func<System.Int32, System.Int32> a,
            Func<System.String, System.String> b,
            Func<System.Decimal, System.Decimal> c,
            Func<System.Int32?, System.Int32?> d,
            Func<Gundi.Tests.Record?, Gundi.Tests.Record?> nullableRecord
            )
        {
            return tag switch
            {
                1 => A(a(this.a)),
                2 => B(b(this.b)),
                3 => C(c(this.c)),
                4 => D(d(this.d)),
                5 => NullableRecord(nullableRecord(this.nullableRecord)),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }        

        private string GetCaseToString()
        {
            return tag switch
            {
                1 => this.a.ToString(),
                2 => this.b.ToString(),
                3 => this.c.ToString(),
                4 => this.d?.ToString() ?? "null",
                5 => this.nullableRecord?.ToString() ?? "null",
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }

        public override string ToString() => $"{ActualCaseName()} {GetCaseToString()}";

        
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

        
        public TOut MatchNullableRecord<TOut>(Func<Gundi.Tests.Record?, TOut> nullableRecord, TOut _)
            => IsNullableRecord() ? nullableRecord(this.nullableRecord) : _;
            
        public TOut MatchNullableRecord<TOut>(Func<Gundi.Tests.Record?, TOut> nullableRecord, Func<TOut> _)
            => IsNullableRecord() ? nullableRecord(this.nullableRecord) : _();
                    
        public Gundi.Tests.Record? CastToNullableRecord()
            => IsNullableRecord() 
                ? this.nullableRecord 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: NullableRecord, Actual state: {ActualCaseName()}.");

        
        public class Converter : JsonConverter<SimpleUnion>
        {
            public override SimpleUnion NullValue => default!;
            public override JsonUnion MapToJsonUnion(SimpleUnion value)
            {
                var obj = value.Match(
                    a => a as object,
                    b => b as object,
                    c => c as object,
                    d => d as object,
                    nullableRecord => nullableRecord as object
                    );
                return new JsonUnion(value.ActualCaseName(),  new [] { obj! });
            }
        
            public override SimpleUnion UnionResolver(string caseName, Func<Type, object> deserialize)
            {
                return caseName switch
                {
                    "A" => A((System.Int32)deserialize(typeof(System.Int32)!)),
                    "B" => B((System.String)deserialize(typeof(System.String)!)),
                    "C" => C((System.Decimal)deserialize(typeof(System.Decimal)!)),
                    "D" => D((System.Int32?)deserialize(typeof(System.Int32?)!)),
                    "NullableRecord" => NullableRecord((Gundi.Tests.Record?)deserialize(typeof(Gundi.Tests.Record?)!)),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), caseName, "Union has undefined state!")
                };
            }
        }
        
        public class SystemConverter : UnionJsonConverter<SimpleUnion>
        {
            public SystemConverter() : base(new Converter()) { }
        }
        
        public class JsonNetConverter : UnionJsonNetConverter<SimpleUnion>
        {
            public JsonNetConverter() : base(new Converter()) { }
        }
    }
}