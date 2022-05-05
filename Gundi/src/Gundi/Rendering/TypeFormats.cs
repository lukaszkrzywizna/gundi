using Microsoft.CodeAnalysis;

namespace Gundi.Rendering;

internal static class TypeFormats
{
    public static readonly SymbolDisplayFormat RecordTypeFormat = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        genericsOptions: (SymbolDisplayGenericsOptions) 7,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    public static readonly SymbolDisplayFormat ParameterTypeFormat = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: (SymbolDisplayGenericsOptions) 7);
    
    public static readonly SymbolDisplayFormat ParameterNullableTypeFormat = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: (SymbolDisplayGenericsOptions) 7,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    public static readonly SymbolDisplayFormat TypeNameOnlyWithoutGeneric = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        genericsOptions: SymbolDisplayGenericsOptions.None);

    public static readonly SymbolDisplayFormat TypeNameOnlyWithSimpleGeneric = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);
}