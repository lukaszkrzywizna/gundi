using Microsoft.CodeAnalysis;

namespace Gundi;

internal static class SettingsAnalyzer
{
    public static AnalyzerResult<UnionAnalyzerSettings> GetUnionSettings(INamedTypeSymbol unionType)
    {
        var attribute =
            unionType.GetAttributes()
                .Single(x => x.AttributeClass?.ToDisplayString() == UnionGenConsts.UnionAttributeName);

        var customException = GetCustomExceptionSettings(attribute);
        var ignoreJsonAttribute = GetJsonConvertAttribute(attribute);
        return AnalyzerResult.Compose(customException, ignoreJsonAttribute, 
            (ex, c) => new UnionAnalyzerSettings(ex, c));
    }

    private static AnalyzerResult<INamedTypeSymbol?> GetCustomExceptionSettings(AttributeData attribute)
    {
        INamedTypeSymbol? typeSymbol = null;
        var attrEx = attribute.NamedArguments
            .SingleOrDefault(x => x.Key == nameof(UnionAttribute.CustomCastException)).Value;
        if (!attrEx.IsNull && attrEx.Value is INamedTypeSymbol exSymbol)
        {
            var exCtor = exSymbol.Constructors.Single();
            var match = CheckExceptionBaseType(exSymbol) && exCtor.Parameters.All(x => x.Type.Name == nameof(String)) &&
                        exCtor.Parameters.Length == 2; //can't be generic
            typeSymbol = exSymbol;
        }

        return AnalyzerResult.NoDiagnose(typeSymbol);

        bool CheckExceptionBaseType(ITypeSymbol s)
        {
            return s.BaseType is not null &&
                   (s.BaseType.Name == nameof(Exception) || CheckExceptionBaseType(s.BaseType));
        }
    }
    private static AnalyzerResult<bool> GetJsonConvertAttribute(AttributeData attribute)
    {
        const string jsonSetting = nameof(UnionAttribute.IgnoreJsonConverterAttribute);
        var includeAttribute = true;
        var attr = attribute.NamedArguments.SingleOrDefault(x => x.Key == jsonSetting).Value;
        if (!attr.IsNull && attr.Value is bool ignoreAttribute) includeAttribute = ignoreAttribute == false;
        return AnalyzerResult.NoDiagnose(includeAttribute);
    }
}

internal record UnionAnalyzerSettings(INamedTypeSymbol? CustomException, bool IncludeJsonAttribute) 
    : UnionSettings<INamedTypeSymbol?>(CustomException, IncludeJsonAttribute);

internal record UnionSettings<T>(T CustomException, bool IncludeJsonAttribute)
{
    public UnionSettings<TOut> Map<TOut>(Func<T, TOut> mapper) => new(mapper(CustomException), IncludeJsonAttribute);
}