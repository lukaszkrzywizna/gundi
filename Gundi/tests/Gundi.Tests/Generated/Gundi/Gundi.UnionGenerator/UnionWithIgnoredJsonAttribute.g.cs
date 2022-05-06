#nullable enable 

using System;
using Gundi;

namespace Gundi.Tests
{
    
    
    partial record UnionWithIgnoredJsonAttribute
    {
        private readonly byte tag;
        
        private readonly (System.Int32, System.String) a;
        private readonly System.String b;
        
        public bool IsA() => tag == 1;
        public bool IsB() => tag == 2;

        private UnionWithIgnoredJsonAttribute(
            (System.Int32, System.String) a,
            System.String b,
            byte tag)
        {
            this.a = a;
            this.b = b;
            this.tag = tag;
        }
        
        public static UnionWithIgnoredJsonAttribute A((System.Int32, System.String) a) 
            => new(
                a,
                default!,
                1
            );
        public static UnionWithIgnoredJsonAttribute B(System.String b) 
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
            Action<(System.Int32, System.String)> a,
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
            Func<(System.Int32, System.String), TOut> a,
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
        
        public UnionWithIgnoredJsonAttribute Map(
            Func<(System.Int32, System.String), (System.Int32, System.String)> a,
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

        public override int GetHashCode()
        {
            return tag switch
            {
                1 => System.HashCode.Combine(this.a),
                2 => System.HashCode.Combine(this.b),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }

        public virtual bool Equals(UnionWithIgnoredJsonAttribute? other)
        {
            return tag switch
            {
                _ when other is null => false,
                1 => other.IsA() && System.Collections.Generic.EqualityComparer<(System.Int32, System.String)>.Default.Equals(this.a, other.CastToA()),
                2 => other.IsB() && System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(this.b, other.CastToB()),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }

        
        public TOut MatchA<TOut>(Func<(System.Int32, System.String), TOut> a, TOut _)
            => IsA() ? a(this.a) : _;
            
        public TOut MatchA<TOut>(Func<(System.Int32, System.String), TOut> a, Func<TOut> _)
            => IsA() ? a(this.a) : _();
                    
        public (System.Int32, System.String) CastToA()
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

        
        public class Converter : JsonConverter<UnionWithIgnoredJsonAttribute>
        {
            public override UnionWithIgnoredJsonAttribute NullValue => default!;
            public override JsonUnion MapToJsonUnion(UnionWithIgnoredJsonAttribute value)
            {
                var obj = value.Match(
                    a => a as object,
                    b => b as object
                    );
                return new JsonUnion(value.ActualCaseName(),  new [] { obj! });
            }
        
            public override UnionWithIgnoredJsonAttribute UnionResolver(string caseName, Func<Type, object> deserialize)
            {
                return caseName switch
                {
                    "A" => A(((System.Int32, System.String))deserialize(typeof((System.Int32, System.String))!)),
                    "B" => B((System.String)deserialize(typeof(System.String)!)),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), caseName, "Union has undefined state!")
                };
            }
        }
        
        public class SystemConverter : UnionJsonConverter<UnionWithIgnoredJsonAttribute>
        {
            public SystemConverter() : base(new Converter()) { }
        }
        
        public class JsonNetConverter : UnionJsonNetConverter<UnionWithIgnoredJsonAttribute>
        {
            public JsonNetConverter() : base(new Converter()) { }
        }
    }
}