namespace Gundi;

public abstract class JsonConverter<T>
{
    public abstract JsonUnion MapToJsonUnion(T value);
    public abstract T UnionResolver(string caseName, Func<Type, object> deserialize);
    public abstract T? NullValue { get; }
}