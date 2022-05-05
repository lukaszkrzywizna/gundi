#nullable enable 

using System;
using Gundi;

namespace Gundi.Tests
{
    [System.Text.Json.Serialization.JsonConverter(typeof(UnionJsonConverterFactory))]
    [Newtonsoft.Json.JsonConverter(typeof(UnionJsonNetConverter))]
    partial record GenericStructNotNullable<T> where T : struct
    {
        private readonly byte tag;
        
        private readonly T value;
        
        public bool IsValue() => tag == 1;

        private GenericStructNotNullable(
            T value,
            byte tag)
        {
            this.value = value;
            this.tag = tag;
        }
        
        public static GenericStructNotNullable<T> Value(T value) 
            => new(
                value,
                1
            );
        
        private string ActualCaseName()
        {
            return tag switch
            {
                1 => "Value",
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public void Match(
            Action<T> value
            )
        {
            switch (tag)
            {
                case 1: 
                    value(this.value);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!");
            };
        }        
        
        public TOut Match<TOut>(
            Func<T, TOut> value
            )
        {
            return tag switch
            {
                1 => value(this.value),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }
        
        public GenericStructNotNullable<T> Map(
            Func<T, T> value
            )
        {
            return tag switch
            {
                1 => Value(value(this.value)),
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }        

        private string GetCaseToString()
        {
            return tag switch
            {
                1 => this.value.ToString() ?? "null",
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Union has undefined state!")
            };
        }

        public override string ToString() => $"{ActualCaseName()} {GetCaseToString()}";

        
        public TOut MatchValue<TOut>(Func<T, TOut> value, TOut _)
            => IsValue() ? value(this.value) : _;
            
        public TOut MatchValue<TOut>(Func<T, TOut> value, Func<TOut> _)
            => IsValue() ? value(this.value) : _();
                    
        public T CastToValue()
            => IsValue() 
                ? this.value 
                : throw new InvalidOperationException(
                    $"Wrong union cast. Expected state: Value, Actual state: {ActualCaseName()}.");

        
        public class Converter : JsonConverter<GenericStructNotNullable<T>>
        {
            public override GenericStructNotNullable<T> NullValue => default!;
            public override JsonUnion MapToJsonUnion(GenericStructNotNullable<T> value)
            {
                var obj = value.Match(
                    value => value as object
                    );
                return new JsonUnion(value.ActualCaseName(),  new [] { obj! });
            }
        
            public override GenericStructNotNullable<T> UnionResolver(string caseName, Func<Type, object> deserialize)
            {
                return caseName switch
                {
                    "Value" => Value((T)deserialize(typeof(T)!)),
                    _ => throw new ArgumentOutOfRangeException(nameof(JsonUnion.Case), caseName, "Union has undefined state!")
                };
            }
        }
        
        public class SystemConverter : UnionJsonConverter<GenericStructNotNullable<T>>
        {
            public SystemConverter() : base(new Converter()) { }
        }
        
        public class JsonNetConverter : UnionJsonNetConverter<GenericStructNotNullable<T>>
        {
            public JsonNetConverter() : base(new Converter()) { }
        }
    }
}