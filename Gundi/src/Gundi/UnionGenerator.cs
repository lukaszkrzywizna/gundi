using System.Collections.Immutable;
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
            .Select((s, token) => s!.Value);

        var compilationResult = context.CompilationProvider.Combine(foundRecords.Collect());

        context.RegisterSourceOutput(compilationResult,
            static (ctx, source) => Execute(source.Left, source.Right, ctx));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is RecordDeclarationSyntax {AttributeLists.Count: > 0};

    private record struct SemanticTarget(INamedTypeSymbol Symbol, RecordDeclarationSyntax Syntax);

    private static SemanticTarget? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var recordSyntax = (RecordDeclarationSyntax) context.Node;

        if (recordSyntax.AttributeLists.Any() == false) return null;
        var symbol = context.SemanticModel.GetDeclaredSymbol(recordSyntax);
        if (symbol is not INamedTypeSymbol namedSymbol) return null;
        if (namedSymbol.GetAttributes()
            .Any(a => a.AttributeClass?.ToDisplayString() == UnionGenConsts.UnionAttributeName))
            return new SemanticTarget(namedSymbol, recordSyntax);

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<SemanticTarget> foundRecords,
        SourceProductionContext context)
    {
        if (foundRecords.IsDefaultOrEmpty) return;
        foreach (var union in foundRecords.Select(x => GenerateUnion(x.Symbol)).Where(x => x is not null))
            context.AddSource($"{union!.FileName}.g.cs", union.Script);
    }

    private record GeneratedUnion(string FileName, string Script);

    private static GeneratedUnion? GenerateUnion(INamedTypeSymbol symbol)
    {
        var settings = SettingsAnalyzer.GetUnionSettings(symbol);
        var anyError = settings.Diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error);

        if (anyError)
            return null;

        var cases = Analyzer.GetUnionCases(symbol);

        var namespaceType = symbol.ContainingNamespace.ToDisplayString();

        var input = new TemplateInput(
            new Union(
                namespaceType, symbol.ToDisplayString(TypeFormats.RecordTypeFormat),
                symbol.ToDisplayString(TypeFormats.SimpleTypeFormat),
                symbol.ToDisplayString(TypeFormats.TypeWithSimpleGeneric),
                settings.Result.Map(x =>
                    new TypeAttribute(x?.ToDisplayString(TypeFormats.ParameterTypeFormat) ?? string.Empty,
                        x is not null)),
                cases
                    .Select((x, i) =>
                        new Case(i + 1, x.Type.ToDisplayString(TypeFormats.ParameterTypeFormat), x.Name,
                            x.Name.FirstCharToUpper()))
                    .ToArray()
            )
        );

        var output = TemplateRender.Render(input);
        var fileName = input.Union.TypeNameOnly + (symbol.IsGenericType ? "T" : string.Empty);
        return new GeneratedUnion(fileName, output);
    }
}