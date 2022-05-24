using Microsoft.CodeAnalysis;

namespace Gundi.Analyzers;

public static class CodeInspection
{
    public static AttributeData GetAttribute(this INamedTypeSymbol type, string name)
    {
        return type.GetAttributes()
            .First(x => x.AttributeClass?.ToDisplayString() == name);
    }

    public static TypedConstant? FindAttributeParameter(this AttributeData attribute, string name)
    {
        var found = attribute.NamedArguments.SingleOrDefault(x => x.Key == name).Value;
        return found.IsNull ? null : found;
    }

    public static bool HasConstructorOfTypes(this INamedTypeSymbol type, params Type[] orderedTypes)
    {
        return type.Constructors.Any(x => x.Parameters.Length == orderedTypes.Length &&
                                          x.Parameters
                                              .Select((s, i) => s.Type.Name == orderedTypes[i].Name)
                                              .All(a => a));
    } 
    
    // For some types `ToString()` returns nullable string
    public static bool HasNullableToString(this ITypeSymbol type)
        => type switch
        {
            { SpecialType: SpecialType.System_Object } => true,
            { SpecialType: SpecialType.System_Nullable_T } => true,
            { SpecialType: SpecialType.System_String } => false,
            { SpecialType: SpecialType.System_DateTime } => false,
            { SpecialType: SpecialType.System_Int16 } => false,
            { SpecialType: SpecialType.System_UInt16 } => false,
            { SpecialType: SpecialType.System_Int32 } => false,
            { SpecialType: SpecialType.System_UInt32 } => false,
            { SpecialType: SpecialType.System_Int64 } => false,
            { SpecialType: SpecialType.System_UInt64 } => false,
            { SpecialType: SpecialType.System_Decimal } => false,
            { SpecialType: SpecialType.System_Single } => false,
            { SpecialType: SpecialType.System_Double } => false,
            { SpecialType: SpecialType.System_Enum } => false,
            { SpecialType: SpecialType.System_Boolean } => false,
            { SpecialType: SpecialType.System_Char } => false,
            { SpecialType: SpecialType.System_SByte } => false,
            { SpecialType: SpecialType.System_Byte } => false,
            { SpecialType: SpecialType.System_IntPtr } => false,
            { SpecialType: SpecialType.System_UIntPtr } => false,
            _ => type.FindToStringMethod()?.ReturnNullableAnnotation != NullableAnnotation.NotAnnotated,
        };

    internal static IMethodSymbol? FindToStringMethod(this ITypeSymbol type) =>
        FindMethod(type, m => m.Parameters.IsEmpty && m.Name == nameof(ToString)
                                                   && m.ReturnType.SpecialType == SpecialType.System_String);

    internal static IMethodSymbol? FindMethod(ITypeSymbol type, Func<IMethodSymbol, bool> pred)
        => type
            .GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(pred);
    
    internal static IMethodSymbol? FindStaticPartialMth(this ITypeSymbol typeSymbol, string name)
    {
        var mth = typeSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsPartialDefinition && m.MethodKind == MethodKind.Ordinary &&
                                 m.Name == name);

        return mth;
    }

    internal static IReadOnlyCollection<IParameterSymbol> GetParametersOrDefault(this IMethodSymbol? symbol) => 
        symbol?.Parameters ?? (IReadOnlyCollection<IParameterSymbol>) Array.Empty<IParameterSymbol>();
}