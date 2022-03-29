using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Gundi;

internal static class Analyzer
{
    private const string UnionCasesMethod = "Cases";

    internal static ImmutableArray<IParameterSymbol> GetUnionCases(ITypeSymbol typeSymbol)
    {
        var mth = typeSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsPartialDefinition && m.MethodKind == MethodKind.Ordinary &&
                                 m.Name == UnionCasesMethod);

        return mth?.Parameters ?? ImmutableArray<IParameterSymbol>.Empty;
    }
}