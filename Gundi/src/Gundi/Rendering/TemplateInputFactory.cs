using Gundi.Extensions;
using Gundi.Settings;
using Gundi.Unions;
using Microsoft.CodeAnalysis;

namespace Gundi.Rendering;

internal record GeneratorData(AnalyzedUnion Union, UnionAnalyzerSettings Settings, NullableContext NullableContext);

public static class TemplateInputFactory
{
    internal static TemplateInput Build(GeneratorData data)
    {
        var namespaceType = data.Union.Type.ContainingNamespace.ToDisplayString();
        
        return new TemplateInput(
            new Union(
                namespaceType, data.Union.Type.ToDisplayString(TypeFormats.RecordTypeFormat),
                data.Union.Type.ToDisplayString(TypeFormats.TypeNameOnlyWithoutGeneric),
                data.Union.Type.ToDisplayString(TypeFormats.TypeNameOnlyWithSimpleGeneric),
                data.NullableContext.HasFlag(NullableContext.AnnotationsEnabled),
                data.Settings.Map(x =>
                    new TypeAttribute(x?.ToDisplayString(TypeFormats.ParameterTypeFormat) ?? string.Empty,
                        x is not null)),
                data.Union.Cases
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
                                       x.Type is ITypeParameterSymbol { HasValueTypeConstraint: false },
                            HasNullableToString: x.Type.HasNullableToString()
                        );
                    })
                    .ToArray()
            )
        );
    }
}