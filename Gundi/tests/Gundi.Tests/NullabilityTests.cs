using Xunit;

namespace Gundi.Tests;

#nullable disable
[Union]
public partial record GenericStruct<T> where T : struct
{
    static partial void Cases(T value);
}
#nullable enable

[Union]
public partial record GenericStructNotNullable<T> where T : struct
{
    static partial void Cases(T value);
}

public class NullabilityTests
{
    [Fact]
    public void AllUnionTypes_ToStringIsGenerated()
    {
        var u = GenericStructNotNullable<int>.Value(5);
        Assert.Equal("Value 5", u.ToString());
        var us = GenericStruct<int>.Value(5);
        Assert.Equal("Value 5", us.ToString());
        var su = SimpleUnion.A(5);
        Assert.Equal("A 5", su.ToString());
        var nu = UnionWithGeneric<string?>.Generic(null);
        Assert.Equal("Generic null", nu.ToString());
    }
}