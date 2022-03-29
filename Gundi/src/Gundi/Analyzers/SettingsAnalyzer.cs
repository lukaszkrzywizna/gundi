using Microsoft.CodeAnalysis;

namespace Gundi;

internal static class SettingsAnalyzer
{
    public static AnalyzerResult<UnionSettings<INamedTypeSymbol?>> GetUnionSettings(INamedTypeSymbol unionType)
    {
        var attribute =
            unionType.GetAttributes()
                .Single(x => x.AttributeClass?.ToDisplayString() == UnionGenConsts.UnionAttributeName);

        var customException = GetCustomExceptionSettings(attribute);
        var caseAttribute = GetUnionAttribute(attribute, nameof(UnionAttribute.FieldAttribute));
        var ctorAttribute = GetUnionAttribute(attribute, nameof(UnionAttribute.ConstructorAttribute));

        return AnalyzerResult.Compose(customException, caseAttribute, ctorAttribute,
            (ex, c, ct) => new UnionSettings<INamedTypeSymbol?>(ex, c, ct));
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

    private static AnalyzerResult<INamedTypeSymbol?> GetUnionAttribute(AttributeData attribute, string attributeName)
    {
        INamedTypeSymbol? typeSymbol = null;
        var attr = attribute.NamedArguments.SingleOrDefault(x => x.Key == attributeName).Value;
        if (!attr.IsNull && attr.Value is INamedTypeSymbol s)
        {
            //must be an attribute
            typeSymbol = s;
        }

        return AnalyzerResult.NoDiagnose(typeSymbol);
    }
}

internal record UnionSettings<T>(T CustomException, T CaseAttribute, T ConstructorAttribute)
{
    public UnionSettings<TOut> Map<TOut>(Func<T, TOut> mapper)
        => new(mapper(CustomException), mapper(CaseAttribute), mapper(ConstructorAttribute));
}