using Microsoft.CodeAnalysis;

namespace Gundi.Analyzers;

internal static class DiagnoseFactory
{
    public static Diagnostic Error(string id, string title, string message, Location location)
        => CreateDiagnose(id, title, message, location, DiagnosticSeverity.Error,
            WellKnownDiagnosticTags.NotConfigurable);

    private static Diagnostic CreateDiagnose(string id, string title, string message, Location location,
        DiagnosticSeverity severity, params string[] customTags)
        => Diagnostic.Create(
            new DiagnosticDescriptor(
                $"{nameof(Gundi)}.{id}",
                title,
                message,
                nameof(Gundi),
                severity,
                true,
                customTags: customTags),
            location);
}