using Microsoft.CodeAnalysis;

namespace Gundi;

internal record AnalyzerResult<T>(IReadOnlyCollection<Diagnostic> Diagnostics, T Result)
{
    public AnalyzerResult<TOut> Map<TOut>(Func<T, TOut> mapper) => new(Diagnostics, mapper(Result));
}

internal static class AnalyzerResult
{
    public static AnalyzerResult<T> NoDiagnose<T>(T value) => new(Array.Empty<Diagnostic>(), value);

    public static AnalyzerResult<T> Compose<T1, T2, T>(AnalyzerResult<T1> a1, AnalyzerResult<T2> a2,
        Func<T1, T2, T> construct)
    {
        var diagnostics = a1.Diagnostics.Concat(a2.Diagnostics);
        var value = construct(a1.Result, a2.Result);
        return new AnalyzerResult<T>(diagnostics.ToArray(), value);
    }

    public static AnalyzerResult<T> New<T>(IReadOnlyCollection<Diagnostic> diagnostics, T value) =>
        new(diagnostics, value);
}