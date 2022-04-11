using Xunit;

namespace Gundi.Tests;

public class ToStringTests
{
    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { ComplexUnion.A(new ComplexEntity(1, "test")), "A: ComplexEntity { A = 1, B = test }" },
            new object[] { ComplexUnion.B(SimpleUnion.A(2)), "B: A: 2" },
            new object[] { ComplexUnion.C(null), "C: null" },
            new object[] { ComplexUnion.C(2), "C: 2" },
            new object[] { ComplexUnion.D("test"), "D: test" },
            new object[] { ComplexUnion.E(null), "E: null" },
            new object[] { ComplexUnion.E(new ComplexEntity(1, "test")), "E: ComplexEntity { A = 1, B = test }" }
        };
    
    [Theory]
    [MemberData(nameof(Data))]
    public void UnionToString_GenerateProperStringResult(ComplexUnion union, string expected)
    {
        var result = union.ToString();
        
        Assert.Equal(expected, result);
    }
}

[Union]
public partial record ComplexUnion
{
    static partial void Cases(ComplexEntity a, SimpleUnion b, int? c, string d, ComplexEntity? e);
}

public record ComplexEntity(int A, string B);