#nullable enable 

using System;
using Gundi;

namespace Gundi.Tests
{
    [System.Text.Json.Serialization.JsonConverter(typeof(UnionJsonConverterFactory))]
    [Newtonsoft.Json.JsonConverter(typeof(UnionJsonNetConverter))]
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

        private string GetCaseToString()
        {
            return tag switch
            {
                1 => this.a.ToString(),
                2 => this.b.ToString(),
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
                : throw new Gundi.Tests.MyException(this.GetType(), nameof(A), ActualCaseName());
        
        public TOut MatchB<TOut>(Func<System.String, TOut> b, TOut _)
            => IsB() ? b(this.b) : _;
            
        public TOut MatchB<TOut>(Func<System.String, TOut> b, Func<TOut> _)
            => IsB() ? b(this.b) : _();
                    
        public System.String CastToB()
            => IsB() 
                ? this.b 
                : throw new Gundi.Tests.MyException(this.GetType(), nameof(B), ActualCaseName());
        
        public class Converter : JsonConverter<UnionWithCustomException>
        {
            public override UnionWithCustomException NullValue => default!;
            public override JsonUnion MapToJsonUnion(UnionWithCustomException value)
            {
                var obj = value.Match(
                    a => a as object,
                    b => b as object
                    );
                return new JsonUnion(value.ActualCaseName(),  new [] { obj! });
            }
        
            public override UnionWithCustomException UnionResolver(string caseName, Func<Type, object> deserialize)
            {
                return caseName switch
                {
                    "A" => A((System.Int32)deserialize(typeof(System.Int32)!)),
                    "B" => B((System.String)deserialize(typeof(System.String)!)),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), caseName, "Union has undefined state!")
                };
            }
        }
        
        public class SystemConverter : UnionJsonConverter<UnionWithCustomException>
        {
            public SystemConverter() : base(new Converter()) { }
        }
        
        public class JsonNetConverter : UnionJsonNetConverter<UnionWithCustomException>
        {
            public JsonNetConverter() : base(new Converter()) { }
        }
    }
}