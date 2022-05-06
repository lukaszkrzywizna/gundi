using Gundi.Analyzers;
using Gundi.Analyzers.Settings;
using Gundi.Analyzers.Unions;
using Gundi.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gundi.Rendering;

internal record GeneratorData(AnalyzedUnion Union, UnionAnalyzerSettings Settings);

public static class TemplateInputFactory
{
    internal static TemplateInput Build(GeneratorData data)
    {
        var namespaceType = data.Union.Target.Symbol.ContainingNamespace.ToDisplayString();

        var union = new Union(
            Namespace: namespaceType,
            FullDefinitionType: data.Union.Target.Symbol.ToDisplayString(TypeFormats.RecordTypeFormat),
            TypeNameOnly: data.Union.Target.Symbol.ToDisplayString(TypeFormats.TypeNameOnlyWithoutGeneric),
            TypeWithSimpleGeneric: data.Union.Target.Symbol.ToDisplayString(TypeFormats.TypeNameOnlyWithSimpleGeneric),
            Cases: data.Union.Cases
                .Select((x, i) =>
                {
                    var nullable = x.Type.ToDisplayString(TypeFormats.ParameterNullableTypeFormat);
                    return new Case(
                        Index: i + 1,
                        Type: nullable,
                        NotNullableType: x.Type.ToDisplayString(TypeFormats.ParameterTypeFormat),
                        Name: x.Name,
                        PascalName: x.Name.FirstCharToUpper(),
                        CanBeNull: nullable[nullable.Length - 1] == '?' ||
                                   x.Type is ITypeParameterSymbol {HasValueTypeConstraint: false},
                        HasNullableToString: x.Type.HasNullableToString()
                    );
                }).ToArray()
        );
        
        var settings = data.Settings.Map(x =>
            new TypeAttribute(x?.ToDisplayString(TypeFormats.ParameterTypeFormat) ?? string.Empty,
                x is not null)); 
        
        return new TemplateInput(union, settings, 
            NullableEnabled: data.Union.Target.NullableContext.HasFlag(NullableContext.AnnotationsEnabled)
        );
    }
}