#nullable enable 

using System;
using Gundi;

namespace Gundi.Tests
{
    [System.Text.Json.Serialization.JsonConverter(typeof(UnionJsonConverterFactory))]
    [Newtonsoft.Json.JsonConverter(typeof(UnionJsonNetConverter))]
    partial record CSharpUnion
    {
        private readonly byte tag;
        
        private readonly System.Int32 a;
        private readonly System.String b;
        private readonly Gundi.Tests.Record f;
        private readonly (System.String, System.Int32) t;
        
        public bool IsA() => tag == 1;
        public bool IsB() => tag == 2;
        public bool IsF() => tag == 3;
        public bool IsT() => tag == 4;

        private CSharpUnion(
            System.Int32 a,
            System.String b,
            Gundi.Tests.Record f,
            (System.String, System.Int32) t,
            byte tag)
        {
            this.a = a;
            this.b = b;
            this.f = f;
            this.t = t;
            this.tag = tag;
        }
        
        public static CSharpUnion A(System.Int32 a) 
            => new(
                a,
                default!,
                default!,
                default!,
                1
            );
        public static CSharpUnion B(System.String b) 
            => new(
                default!,
                b,
                default!,
                default!,
                2
            );
        public static CSharpUnion F(Gundi.Tests.Record f) 
            => new(
                default!,
                default!,
                f,
                default!,
                3
            );
        public static CSharpUnion T((System.String, System.Int32) t) 
            => new(
                default!,
                default!,
                default!,
                t,
                4
            );
        
        private string ActualCaseName()
        {
            return tag switch
            {
                1 => "A",
                2 => "B",
                3 => "F",
                4 => "T",
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public void Match(
            Action<System.Int32> a,
            Action<System.String> b,
            Action<Gundi.Tests.Record> f,
            Action<(System.String, System.Int32)> t
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
                    f(this.f);
                    break;
                case 4: 
                    t(this.t);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!");
            };
        }        
        
        public TOut Match<TOut>(
            Func<System.Int32, TOut> a,
            Func<System.String, TOut> b,
            Func<Gundi.Tests.Record, TOut> f,
            Func<(System.String, System.Int32), TOut> t
            )
        {
            return tag switch
            {
                1 => a(this.a),
                2 => b(this.b),
                3 => f(this.f),
                4 => t(this.t),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public CSharpUnion Map(
            Func<System.Int32, System.Int32> a,
            Func<System.String, System.String> b,
            Func<Gundi.Tests.Record, Gundi.Tests.Record> f,
            Func<(System.String, System.Int32), (System.String, System.Int32)> t
            )
        {
            return tag switch
            {
                1 => A(a(this.a)),
                2 => B(b(this.b)),
                3 => F(f(this.f)),
                4 => T(t(this.t)),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }        

        private string GetCaseToString()
        {
            return tag switch
            {
                1 => this.a.ToString(),
                2 => this.b.ToString(),
                3 => this.f.ToString() ?? "null",
                4 => this.t.ToString(),
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

        
        public TOut MatchF<TOut>(Func<Gundi.Tests.Record, TOut> f, TOut _)
            => IsF() ? f(this.f) : _;
            
        public TOut MatchF<TOut>(Func<Gundi.Tests.Record, TOut> f, Func<TOut> _)
            => IsF() ? f(this.f) : _();
                    
        public Gundi.Tests.Record CastToF()
            => IsF() 
                ? this.f 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: F, Actual state: {ActualCaseName()}.");

        
        public TOut MatchT<TOut>(Func<(System.String, System.Int32), TOut> t, TOut _)
            => IsT() ? t(this.t) : _;
            
        public TOut MatchT<TOut>(Func<(System.String, System.Int32), TOut> t, Func<TOut> _)
            => IsT() ? t(this.t) : _();
                    
        public (System.String, System.Int32) CastToT()
            => IsT() 
                ? this.t 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: T, Actual state: {ActualCaseName()}.");

        
        public class Converter : JsonConverter<CSharpUnion>
        {
            public override CSharpUnion NullValue => default!;
            public override JsonUnion MapToJsonUnion(CSharpUnion value)
            {
                var obj = value.Match(
                    a => a as object,
                    b => b as object,
                    f => f as object,
                    t => t as object
                    );
                return new JsonUnion(value.ActualCaseName(),  new [] { obj! });
            }
        
            public override CSharpUnion UnionResolver(string caseName, Func<Type, object> deserialize)
            {
                return caseName switch
                {
                    "A" => A((System.Int32)deserialize(typeof(System.Int32)!)),
                    "B" => B((System.String)deserialize(typeof(System.String)!)),
                    "F" => F((Gundi.Tests.Record)deserialize(typeof(Gundi.Tests.Record)!)),
                    "T" => T(((System.String, System.Int32))deserialize(typeof((System.String, System.Int32))!)),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), caseName, "Union has undefined state!")
                };
            }
        }
        
        public class SystemConverter : UnionJsonConverter<CSharpUnion>
        {
            public SystemConverter() : base(new Converter()) { }
        }
        
        public class JsonNetConverter : UnionJsonNetConverter<CSharpUnion>
        {
            public JsonNetConverter() : base(new Converter()) { }
        }
    }
}