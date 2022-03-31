namespace Gundi;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UnionAttribute : Attribute
{
    public Type? CustomCastException { get; set; }

    public UnionAttribute()
    {
    }
}