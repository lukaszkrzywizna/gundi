using Xunit;

namespace Gundi.Tests;

public class EqualityTests
{
    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { ComplexUnion<int, string>.A(new ComplexEntity(1, "test")), ComplexUnion<int, string>.A(new ComplexEntity(2, "test")), false },
            new object[] { ComplexUnion<int, string>.A(new ComplexEntity(1, "test")), ComplexUnion<int, string>.A(new ComplexEntity(1, "test")), true },
            new object[] { ComplexUnion<int, string>.B(SimpleUnion.A(2)), ComplexUnion<int, string>.B(SimpleUnion.A(3)), false },
            new object[] { ComplexUnion<int, string>.B(SimpleUnion.A(2)), ComplexUnion<int, string>.B(SimpleUnion.A(2)), true },
            new object[] { ComplexUnion<int, string>.C(null), ComplexUnion<int, string>.C(null), true },
            new object[] { ComplexUnion<int, string>.C(2), ComplexUnion<int, string>.C(2), true },
            new object[] { ComplexUnion<int, string>.D("test"), ComplexUnion<int, string>.D("test"), true },
            new object[] { ComplexUnion<int, string>.StructGen(5), ComplexUnion<int, string>.StructGen(5), true},
            new object[] { ComplexUnion<int, string?>.ClassGen(null), ComplexUnion<int, string?>.ClassGen(null), true }
        };
    
    [Theory]
    [MemberData(nameof(Data))]
    public void Do(ComplexUnion<int, string> union, ComplexUnion<int, string> union2, bool equal)
    {
        Assert.True(union.Equals(union2) == equal);
        Assert.True(union.Equals((object)union2) == equal);
        Assert.True(union.GetHashCode() == union2.GetHashCode() == equal);
        Assert.True(union == union2 == equal);
        Assert.True(EqualityComparer<ComplexUnion<int, string>>.Default.Equals(union, union2) == equal);
    }
}