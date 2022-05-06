using Xunit;

namespace Gundi.Tests;

public class ToStringTests
{
    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { ComplexUnion<int, string>.A(new ComplexEntity(1, "test")), "A ComplexEntity { A = 1, B = test }" },
            new object[] { ComplexUnion<int, string>.B(SimpleUnion.A(2)), "B A 2" },
            new object[] { ComplexUnion<int, string>.C(null), "C null" },
            new object[] { ComplexUnion<int, string>.C(2), "C 2" },
            new object[] { ComplexUnion<int, string>.D("test"), "D test" },
            new object[] { ComplexUnion<int, string>.E(null), "E null" },
            new object[] { ComplexUnion<int, string>.E(new ComplexEntity(1, "test")), "E ComplexEntity { A = 1, B = test }" },
            new object[] { ComplexUnion<int, string>.StructGen(5), "StructGen 5" },
            new object[] { ComplexUnion<int, string?>.ClassGen(null), "ClassGen null" }
        };
    
    [Theory]
    [MemberData(nameof(Data))]
    public void UnionToString_GenerateProperStringResult(ComplexUnion<int, string> union, string expected)
    {
        var result = union.ToString();
        Assert.Equal(expected, result);
    }
}

[Union]
public partial record ComplexUnion<T, T2> where T: struct where T2: class?
{
    static partial void Cases(ComplexEntity a, SimpleUnion b, int? c, string d, ComplexEntity? e, T structGen, T2 classGen);
}

public record ComplexEntity(int A, string B);