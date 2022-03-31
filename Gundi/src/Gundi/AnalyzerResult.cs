using Microsoft.CodeAnalysis;

namespace Gundi;

internal record AnalyzerResult<T>(IReadOnlyCollection<Diagnostic> Diagnostics, T Result)
{
    public AnalyzerResult<TOut> Map<TOut>(Func<T, TOut> mapper) => new(Diagnostics, mapper(Result));

}

internal static class AnalyzerResult
{
    public static AnalyzerResult<T> NoDiagnose<T>(T value) => new(ArraySegment<Diagnostic>.Empty, value);

    public static AnalyzerResult<T> Compose<T1, T2, T3, T>(AnalyzerResult<T1> a1, AnalyzerResult<T2> a2,
        AnalyzerResult<T3> a3, Func<T1, T2, T3, T> construct)
    {
        var diagnostics = a1.Diagnostics.Concat(a2.Diagnostics).Concat(a3.Diagnostics);
        var value = construct(a1.Result, a2.Result, a3.Result);
        return new AnalyzerResult<T>(diagnostics.ToArray(), value);
    }
}