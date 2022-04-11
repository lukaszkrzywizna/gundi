using System.Reflection;
using Scriban;
using Scriban.Runtime;

namespace Gundi;

internal static class TemplateRender
{
    private static readonly Lazy<Template> UnionTemplate = new(() => Template.Parse(LoadTemplateScript("Union")));

    private static string LoadTemplateScript(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var manifestStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{name}.sbn-cs");
        using var reader = new StreamReader(manifestStream!);
        return reader.ReadToEnd();
    }

    public static string Render(TemplateInput input)
    {
        var templateContext = new TemplateContext
        {
            StrictVariables = true,
            MemberRenamer = m => m.Name
        };

        var globals = new ScriptObject();
        globals.Import(input, renamer: m => m.Name);
        templateContext.PushGlobal(globals);
        return UnionTemplate.Value.Render(templateContext);
    }
}

internal record struct Union(string Namespace, string FullDefinitionType, string TypeNameOnly,
    string TypeWithSimpleGeneric, UnionSettings<TypeAttribute> Settings, IReadOnlyCollection<Case> Cases);

internal record struct Case(int Index, string Type, string NotNullableType, string Name, string PascalName, bool CanBeNull);

internal record TemplateInput(Union Union);

internal record TypeAttribute(string? TypeName, bool IsDefined);