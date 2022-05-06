using Microsoft.CodeAnalysis;

namespace Gundi.Analyzers.Unions;

internal record AnalyzedUnion(UnionGenerator.SemanticTarget Target, IReadOnlyCollection<IParameterSymbol> Cases);

internal static class Analyzer
{
    internal static AnalyzerResult<AnalyzedUnion> Analyze(UnionGenerator.SemanticTarget target)
    {
        // TODO: add diagnostics for invalid unions
        var cases = target.Symbol.FindStaticPartialMth(UnionGenConsts.UnionCaseMthName).GetParametersOrDefault();
        return AnalyzerResult.NoDiagnose(new AnalyzedUnion(target, cases));
    }
}