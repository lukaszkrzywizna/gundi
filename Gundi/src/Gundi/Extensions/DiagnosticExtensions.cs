using Microsoft.CodeAnalysis;

namespace Gundi.Extensions;

public static class DiagnosticExtensions
{
    public static void ReportDiagnostics(this SourceProductionContext ctx, IEnumerable<Diagnostic> diagnostics)
    {
        foreach (var diag in diagnostics)
            ctx.ReportDiagnostic(diag);
    }
}