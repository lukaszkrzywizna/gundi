using Microsoft.CodeAnalysis;

namespace Gundi.Unions;

internal record AnalyzedUnion(INamedTypeSymbol Type, IReadOnlyCollection<IParameterSymbol> Cases);

internal static class Analyzer
{
    internal static AnalyzerResult<AnalyzedUnion> Analyze(INamedTypeSymbol symbol)
    {
        // TODO: add diagnostics for invalid unions
        var cases = symbol.FindStaticPartialMth(UnionGenConsts.UnionCaseMthName).GetParametersOrDefault();
        return AnalyzerResult.NoDiagnose(new AnalyzedUnion(symbol, cases));
    }
}