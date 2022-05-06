#nullable enable 

using System;
using Gundi;

namespace Gundi.Tests
{
    [System.Text.Json.Serialization.JsonConverter(typeof(UnionJsonConverterFactory))]
    [Newtonsoft.Json.JsonConverter(typeof(UnionJsonNetConverter))]
    partial record ComplexUnion<T, T2> where T : struct where T2 : class?
    {
        private readonly byte tag;
        
        private readonly Gundi.Tests.ComplexEntity a;
        private readonly Gundi.Tests.SimpleUnion b;
        private readonly System.Int32? c;
        private readonly System.String d;
        private readonly Gundi.Tests.ComplexEntity? e;
        private readonly T structGen;
        private readonly T2 classGen;
        
        public bool IsA() => tag == 1;
        public bool IsB() => tag == 2;
        public bool IsC() => tag == 3;
        public bool IsD() => tag == 4;
        public bool IsE() => tag == 5;
        public bool IsStructGen() => tag == 6;
        public bool IsClassGen() => tag == 7;

        private ComplexUnion(
            Gundi.Tests.ComplexEntity a,
            Gundi.Tests.SimpleUnion b,
            System.Int32? c,
            System.String d,
            Gundi.Tests.ComplexEntity? e,
            T structGen,
            T2 classGen,
            byte tag)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.structGen = structGen;
            this.classGen = classGen;
            this.tag = tag;
        }
        
        public static ComplexUnion<T, T2> A(Gundi.Tests.ComplexEntity a) 
            => new(
                a,
                default!,
                default!,
                default!,
                default!,
                default!,
                default!,
                1
            );
        public static ComplexUnion<T, T2> B(Gundi.Tests.SimpleUnion b) 
            => new(
                default!,
                b,
                default!,
                default!,
                default!,
                default!,
                default!,
                2
            );
        public static ComplexUnion<T, T2> C(System.Int32? c) 
            => new(
                default!,
                default!,
                c,
                default!,
                default!,
                default!,
                default!,
                3
            );
        public static ComplexUnion<T, T2> D(System.String d) 
            => new(
                default!,
                default!,
                default!,
                d,
                default!,
                default!,
                default!,
                4
            );
        public static ComplexUnion<T, T2> E(Gundi.Tests.ComplexEntity? e) 
            => new(
                default!,
                default!,
                default!,
                default!,
                e,
                default!,
                default!,
                5
            );
        public static ComplexUnion<T, T2> StructGen(T structGen) 
            => new(
                default!,
                default!,
                default!,
                default!,
                default!,
                structGen,
                default!,
                6
            );
        public static ComplexUnion<T, T2> ClassGen(T2 classGen) 
            => new(
                default!,
                default!,
                default!,
                default!,
                default!,
                default!,
                classGen,
                7
            );
        
        private string ActualCaseName()
        {
            return tag switch
            {
                1 => "A",
                2 => "B",
                3 => "C",
                4 => "D",
                5 => "E",
                6 => "StructGen",
                7 => "ClassGen",
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public void Match(
            Action<Gundi.Tests.ComplexEntity> a,
            Action<Gundi.Tests.SimpleUnion> b,
            Action<System.Int32?> c,
            Action<System.String> d,
            Action<Gundi.Tests.ComplexEntity?> e,
            Action<T> structGen,
            Action<T2> classGen
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
                    e(this.e);
                    break;
                case 6: 
                    structGen(this.structGen);
                    break;
                case 7: 
                    classGen(this.classGen);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!");
            };
        }        
        
        public TOut Match<TOut>(
            Func<Gundi.Tests.ComplexEntity, TOut> a,
            Func<Gundi.Tests.SimpleUnion, TOut> b,
            Func<System.Int32?, TOut> c,
            Func<System.String, TOut> d,
            Func<Gundi.Tests.ComplexEntity?, TOut> e,
            Func<T, TOut> structGen,
            Func<T2, TOut> classGen
            )
        {
            return tag switch
            {
                1 => a(this.a),
                2 => b(this.b),
                3 => c(this.c),
                4 => d(this.d),
                5 => e(this.e),
                6 => structGen(this.structGen),
                7 => classGen(this.classGen),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public ComplexUnion<T, T2> Map(
            Func<Gundi.Tests.ComplexEntity, Gundi.Tests.ComplexEntity> a,
            Func<Gundi.Tests.SimpleUnion, Gundi.Tests.SimpleUnion> b,
            Func<System.Int32?, System.Int32?> c,
            Func<System.String, System.String> d,
            Func<Gundi.Tests.ComplexEntity?, Gundi.Tests.ComplexEntity?> e,
            Func<T, T> structGen,
            Func<T2, T2> classGen
            )
        {
            return tag switch
            {
                1 => A(a(this.a)),
                2 => B(b(this.b)),
                3 => C(c(this.c)),
                4 => D(d(this.d)),
                5 => E(e(this.e)),
                6 => StructGen(structGen(this.structGen)),
                7 => ClassGen(classGen(this.classGen)),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }        

        private string GetCaseToString()
        {
            return tag switch
            {
                1 => this.a.ToString(),
                2 => this.b.ToString(),
                3 => this.c?.ToString() ?? "null",
                4 => this.d.ToString(),
                5 => this.e?.ToString() ?? "null",
                6 => this.structGen.ToString() ?? "null",
                7 => this.classGen?.ToString() ?? "null",
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
                3 => System.HashCode.Combine(this.c),
                4 => System.HashCode.Combine(this.d),
                5 => System.HashCode.Combine(this.e),
                6 => System.HashCode.Combine(this.structGen),
                7 => System.HashCode.Combine(this.classGen),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }

        public virtual bool Equals(ComplexUnion<T, T2>? other)
        {
            return tag switch
            {
                _ when other is null => false,
                1 => other.IsA() && System.Collections.Generic.EqualityComparer<Gundi.Tests.ComplexEntity>.Default.Equals(this.a, other.CastToA()),
                2 => other.IsB() && System.Collections.Generic.EqualityComparer<Gundi.Tests.SimpleUnion>.Default.Equals(this.b, other.CastToB()),
                3 => other.IsC() && System.Collections.Generic.EqualityComparer<System.Int32?>.Default.Equals(this.c, other.CastToC()),
                4 => other.IsD() && System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(this.d, other.CastToD()),
                5 => other.IsE() && System.Collections.Generic.EqualityComparer<Gundi.Tests.ComplexEntity?>.Default.Equals(this.e, other.CastToE()),
                6 => other.IsStructGen() && System.Collections.Generic.EqualityComparer<T>.Default.Equals(this.structGen, other.CastToStructGen()),
                7 => other.IsClassGen() && System.Collections.Generic.EqualityComparer<T2>.Default.Equals(this.classGen, other.CastToClassGen()),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }

        
        public TOut MatchA<TOut>(Func<Gundi.Tests.ComplexEntity, TOut> a, TOut _)
            => IsA() ? a(this.a) : _;
            
        public TOut MatchA<TOut>(Func<Gundi.Tests.ComplexEntity, TOut> a, Func<TOut> _)
            => IsA() ? a(this.a) : _();
                    
        public Gundi.Tests.ComplexEntity CastToA()
            => IsA() 
                ? this.a 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: A, Actual state: {ActualCaseName()}.");

        
        public TOut MatchB<TOut>(Func<Gundi.Tests.SimpleUnion, TOut> b, TOut _)
            => IsB() ? b(this.b) : _;
            
        public TOut MatchB<TOut>(Func<Gundi.Tests.SimpleUnion, TOut> b, Func<TOut> _)
            => IsB() ? b(this.b) : _();
                    
        public Gundi.Tests.SimpleUnion CastToB()
            => IsB() 
                ? this.b 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: B, Actual state: {ActualCaseName()}.");

        
        public TOut MatchC<TOut>(Func<System.Int32?, TOut> c, TOut _)
            => IsC() ? c(this.c) : _;
            
        public TOut MatchC<TOut>(Func<System.Int32?, TOut> c, Func<TOut> _)
            => IsC() ? c(this.c) : _();
                    
        public System.Int32? CastToC()
            => IsC() 
                ? this.c 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: C, Actual state: {ActualCaseName()}.");

        
        public TOut MatchD<TOut>(Func<System.String, TOut> d, TOut _)
            => IsD() ? d(this.d) : _;
            
        public TOut MatchD<TOut>(Func<System.String, TOut> d, Func<TOut> _)
            => IsD() ? d(this.d) : _();
                    
        public System.String CastToD()
            => IsD() 
                ? this.d 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: D, Actual state: {ActualCaseName()}.");

        
        public TOut MatchE<TOut>(Func<Gundi.Tests.ComplexEntity?, TOut> e, TOut _)
            => IsE() ? e(this.e) : _;
            
        public TOut MatchE<TOut>(Func<Gundi.Tests.ComplexEntity?, TOut> e, Func<TOut> _)
            => IsE() ? e(this.e) : _();
                    
        public Gundi.Tests.ComplexEntity? CastToE()
            => IsE() 
                ? this.e 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: E, Actual state: {ActualCaseName()}.");

        
        public TOut MatchStructGen<TOut>(Func<T, TOut> structGen, TOut _)
            => IsStructGen() ? structGen(this.structGen) : _;
            
        public TOut MatchStructGen<TOut>(Func<T, TOut> structGen, Func<TOut> _)
            => IsStructGen() ? structGen(this.structGen) : _();
                    
        public T CastToStructGen()
            => IsStructGen() 
                ? this.structGen 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: StructGen, Actual state: {ActualCaseName()}.");

        
        public TOut MatchClassGen<TOut>(Func<T2, TOut> classGen, TOut _)
            => IsClassGen() ? classGen(this.classGen) : _;
            
        public TOut MatchClassGen<TOut>(Func<T2, TOut> classGen, Func<TOut> _)
            => IsClassGen() ? classGen(this.classGen) : _();
                    
        public T2 CastToClassGen()
            => IsClassGen() 
                ? this.classGen 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: ClassGen, Actual state: {ActualCaseName()}.");

        
        public class Converter : JsonConverter<ComplexUnion<T, T2>>
        {
            public override ComplexUnion<T, T2> NullValue => default!;
            public override JsonUnion MapToJsonUnion(ComplexUnion<T, T2> value)
            {
                var obj = value.Match(
                    a => a as object,
                    b => b as object,
                    c => c as object,
                    d => d as object,
                    e => e as object,
                    structGen => structGen as object,
                    classGen => classGen as object
                    );
                return new JsonUnion(value.ActualCaseName(),  new [] { obj! });
            }
        
            public override ComplexUnion<T, T2> UnionResolver(string caseName, Func<Type, object> deserialize)
            {
                return caseName switch
                {
                    "A" => A((Gundi.Tests.ComplexEntity)deserialize(typeof(Gundi.Tests.ComplexEntity)!)),
                    "B" => B((Gundi.Tests.SimpleUnion)deserialize(typeof(Gundi.Tests.SimpleUnion)!)),
                    "C" => C((System.Int32?)deserialize(typeof(System.Int32?)!)),
                    "D" => D((System.String)deserialize(typeof(System.String)!)),
                    "E" => E((Gundi.Tests.ComplexEntity?)deserialize(typeof(Gundi.Tests.ComplexEntity)!)),
                    "StructGen" => StructGen((T)deserialize(typeof(T)!)),
                    "ClassGen" => ClassGen((T2)deserialize(typeof(T2)!)),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), caseName, "Union has undefined state!")
                };
            }
        }
        
        public class SystemConverter : UnionJsonConverter<ComplexUnion<T, T2>>
        {
            public SystemConverter() : base(new Converter()) { }
        }
        
        public class JsonNetConverter : UnionJsonNetConverter<ComplexUnion<T, T2>>
        {
            public JsonNetConverter() : base(new Converter()) { }
        }
    }
}