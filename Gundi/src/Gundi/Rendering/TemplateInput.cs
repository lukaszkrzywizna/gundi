using Gundi.Analyzers.Settings;

namespace Gundi.Rendering;

internal record struct Union( 
    string Namespace, 
    string TypeNameOnly, 
    string TypeWithSimpleGeneric,
    string FullDefinitionType, 
    IReadOnlyCollection<Case> Cases);

internal record struct Case(int Index, string Type, string NotNullableType, string Name, string PascalName, 
    bool CanBeNull, bool HasNullableToString);

internal record TemplateInput(Union Union, UnionSettings<TypeAttribute> Settings, bool NullableEnabled);

internal record TypeAttribute(string? TypeName, bool IsDefined);