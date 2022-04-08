using System.Globalization;
using Xunit;

namespace Gundi.Tests;

public class Tests
{
    [Fact]
    public void UnionWithSimpleTypes_FactoryAndCheckMethodsGenerated()
    {
        var a = SimpleUnion.A(5);
        Assert.True(a.IsA());
        Assert.False(a.IsB());
        Assert.False(a.IsC());
        Assert.False(a.IsD());
        var b = SimpleUnion.B("5");
        Assert.True(b.IsB());
        var c = SimpleUnion.C(5);
        Assert.True(c.IsC());
        var d = SimpleUnion.D(null);
        Assert.True(d.IsD());
    }

    [Fact]
    public void UnionWithSimpleTypes_MatchMethodsGenerated()
    {
        var union = SimpleUnion.A(5);
        var mapped = union.Match(
            a => a.ToString(),
            b => b,
            c => c.ToString(CultureInfo.InvariantCulture),
            d => d?.ToString()
            );

        Assert.Equal("5", mapped);
        Assert.Equal("5", union.MatchA(x => x.ToString(), "default"));
        Assert.Equal("default", union.MatchB(x => x, "default"));
        Assert.Equal("default", union.MatchC(x => x.ToString(CultureInfo.InvariantCulture), "default"));
        Assert.Equal("default", union.MatchD(x => x.ToString(), "default"));
    }
    
    [Fact]
    public void UnionWithSimpleTypes_MapMethodGenerated()
    {
        var union = SimpleUnion.A(5);
        var mapped = union.Map(
            a => a,
            b => b,
            c => c,
            d => d);

        Assert.Equal(union, mapped);
    }
    
    [Fact]
    public void UnionWithCustomException_DuringCastThrowsCustomException()
    {
        var unionA = UnionWithCustomException.A(5);
        var exA = Assert.Throws<MyException>(() => unionA.CastToB());
        Assert.Contains(nameof(UnionWithCustomException), exA.Message);
        Assert.Contains($"Expected: {nameof(UnionWithCustomException.B)}", exA.Message);
        Assert.Contains($"Actual: {nameof(UnionWithCustomException.A)}", exA.Message);
        
        var unionB = UnionWithCustomException.B("5");
        var exB = Assert.Throws<MyException>(() => unionB.CastToA());
        Assert.Contains($"Expected: {nameof(UnionWithCustomException.A)}", exB.Message);
        Assert.Contains($"Actual: {nameof(UnionWithCustomException.B)}", exB.Message);
    }
}

[Union]
public partial record SimpleUnion
{
    static partial void Cases(int a, string b, decimal c, int? d);
}

public class BaseException : Exception
{
    public BaseException(Type unionType, string expectedCase, string actualCase) 
        : base($"Wrong {unionType.Name} cast. Expected: {expectedCase}, Actual: {actualCase}")
    {
    }
}

public class MyException : BaseException
{
    public MyException(Type unionType, string expectedCase, string actualCase) 
        : base(unionType, expectedCase, actualCase)
    {
    }
}

[Union(CustomCastException = typeof(MyException))]
public partial record UnionWithCustomException
{
    static partial void Cases(int a, string b);
}