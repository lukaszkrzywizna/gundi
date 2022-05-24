using System.Collections.Immutable;
using Gundi.Analyzers;
using Gundi.Analyzers.Settings;
using Gundi.Extensions;
using Gundi.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gundi;

[Generator]
internal class UnionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DEBUG
        //SpinWait.SpinUntil(() => System.Diagnostics.Debugger.IsAttached);
#endif
        var foundRecords = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)
            .Select((s, _) => s!.Value);

        var compilationResult = context.CompilationProvider.Combine(foundRecords.Collect());

        context.RegisterSourceOutput(compilationResult,
            static (ctx, source) => Execute(source.Left, source.Right, ctx));

        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is RecordDeclarationSyntax {AttributeLists.Count: > 0};
    }

    internal record struct SemanticTarget
        (INamedTypeSymbol Symbol, RecordDeclarationSyntax Syntax, NullableContext NullableContext);

    private static SemanticTarget? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var recordSyntax = (RecordDeclarationSyntax) context.Node;

        if (recordSyntax.AttributeLists.Any() == false) return null;
        var symbol = context.SemanticModel.GetDeclaredSymbol(recordSyntax);
        if (symbol is not INamedTypeSymbol namedSymbol) return null;
        if (namedSymbol.GetAttributes()
            .Any(a => a.AttributeClass?.ToDisplayString() == UnionGenConsts.UnionAttributeName))
            return new SemanticTarget(namedSymbol, recordSyntax,
                context.SemanticModel.GetNullableContext(context.Node.GetLocation().SourceSpan.Start));

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<SemanticTarget> foundRecords,
        SourceProductionContext context)
    {
        if (foundRecords.IsDefaultOrEmpty) return;
        foreach (var result in foundRecords.Select(GenerateUnion))
        {
            context.ReportDiagnostics(result.Diagnostics);
            if (result.Result is not null)
                context.AddSource($"{result.Result.FileName}.g.cs", result.Result.Script);
        }
    }

    private record GeneratedUnion(string FileName, string Script);

    private static AnalyzerResult<GeneratedUnion?> GenerateUnion(SemanticTarget target)
    {
        var settings = Analyzer.GetUnionSettings(target.Symbol);

        var analyzedUnion = Analyzers.Unions.Analyzer.Analyze(target);

        var fullDiagnostics = settings.Diagnostics.Concat(analyzedUnion.Diagnostics).ToArray();
        if (fullDiagnostics.Any(x => x.Severity == DiagnosticSeverity.Error))
            return settings.Map(_ => (GeneratedUnion?) null);

        var input = TemplateInputFactory.Build(
            new GeneratorData(analyzedUnion.Result, settings.Result));
        var output = TemplateRender.Render(input);
        var fileName = input.Union.TypeNameOnly + (target.Symbol.IsGenericType ? "T" : string.Empty);
        return settings.Map<GeneratedUnion?>(_ => new GeneratedUnion(fileName, output));
    }
}