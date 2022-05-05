#nullable enable 

using System;
using Gundi;

namespace Gundi.Tests
{
    [System.Text.Json.Serialization.JsonConverter(typeof(UnionJsonConverterFactory))]
    [Newtonsoft.Json.JsonConverter(typeof(UnionJsonNetConverter))]
    partial record UnionWithGeneric<T> where T : class?
    {
        private readonly byte tag;
        
        private readonly System.Int32? a;
        private readonly System.String b;
        private readonly T generic;
        
        public bool IsA() => tag == 1;
        public bool IsB() => tag == 2;
        public bool IsGeneric() => tag == 3;

        private UnionWithGeneric(
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
        
        public static UnionWithGeneric<T> A(System.Int32? a) 
            => new(
                a,
                default!,
                default!,
                1
            );
        public static UnionWithGeneric<T> B(System.String b) 
            => new(
                default!,
                b,
                default!,
                2
            );
        public static UnionWithGeneric<T> Generic(T generic) 
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
        
        public UnionWithGeneric<T> Map(
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

        private string GetCaseToString()
        {
            return tag switch
            {
                1 => this.a?.ToString() ?? "null",
                2 => this.b.ToString(),
                3 => this.generic?.ToString() ?? "null",
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }

        public override string ToString() => $"{ActualCaseName()} {GetCaseToString()}";

        
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

        
        public class Converter : JsonConverter<UnionWithGeneric<T>>
        {
            public override UnionWithGeneric<T> NullValue => default!;
            public override JsonUnion MapToJsonUnion(UnionWithGeneric<T> value)
            {
                var obj = value.Match(
                    a => a as object,
                    b => b as object,
                    generic => generic as object
                    );
                return new JsonUnion(value.ActualCaseName(),  new [] { obj! });
            }
        
            public override UnionWithGeneric<T> UnionResolver(string caseName, Func<Type, object> deserialize)
            {
                return caseName switch
                {
                    "A" => A((System.Int32?)deserialize(typeof(System.Int32?)!)),
                    "B" => B((System.String)deserialize(typeof(System.String)!)),
                    "Generic" => Generic((T)deserialize(typeof(T)!)),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), caseName, "Union has undefined state!")
                };
            }
        }
        
        public class SystemConverter : UnionJsonConverter<UnionWithGeneric<T>>
        {
            public SystemConverter() : base(new Converter()) { }
        }
        
        public class JsonNetConverter : UnionJsonNetConverter<UnionWithGeneric<T>>
        {
            public JsonNetConverter() : base(new Converter()) { }
        }
    }
}