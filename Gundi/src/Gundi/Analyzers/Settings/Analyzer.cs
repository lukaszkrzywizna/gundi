using Microsoft.CodeAnalysis;

namespace Gundi.Analyzers.Settings;

internal static class Analyzer
{
    public static AnalyzerResult<UnionAnalyzerSettings> GetUnionSettings(INamedTypeSymbol unionType)
    {
        var attribute = unionType.GetAttribute(UnionGenConsts.UnionAttributeName);

        var customException = GetCustomExceptionSettings(attribute);
        var ignoreJsonAttribute = GetJsonConvertAttribute(attribute);
        return AnalyzerResult.Compose(customException, ignoreJsonAttribute, 
            (ex, c) => new UnionAnalyzerSettings(ex, c));
    }

    private static AnalyzerResult<INamedTypeSymbol?> GetCustomExceptionSettings(AttributeData attribute)
    {
        var attrEx = attribute.FindAttributeParameter(nameof(UnionAttribute.CustomCastException));
        if (attrEx?.Value is not INamedTypeSymbol exSymbol) return AnalyzerResult.NoDiagnose<INamedTypeSymbol?>(null);
        
        var hasCtor = exSymbol.TypeHasConstructorWithTwoStringsAndType();
        var inheritsExceptionType = exSymbol.InheritsExceptionType();
        return new AnalyzerResult<INamedTypeSymbol?>(hasCtor.Concat(inheritsExceptionType).ToArray(), exSymbol);
    }
    
    private static AnalyzerResult<bool> GetJsonConvertAttribute(AttributeData attribute)
    {
        var includeAttribute = true;
        var attr = attribute.FindAttributeParameter(UnionGenConsts.IgnoreJson);
        if (attr?.Value is bool ignoreAttribute) includeAttribute = ignoreAttribute == false;
        return AnalyzerResult.NoDiagnose(includeAttribute);
    }
}

internal record UnionAnalyzerSettings(INamedTypeSymbol? CustomException, bool IncludeJsonAttribute) 
    : UnionSettings<INamedTypeSymbol?>(CustomException, IncludeJsonAttribute);

internal record UnionSettings<T>(T CustomException, bool IncludeJsonAttribute)
{
    public UnionSettings<TOut> Map<TOut>(Func<T, TOut> mapper) => new(mapper(CustomException), IncludeJsonAttribute);
}