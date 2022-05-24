using System.Reflection;
using Gundi.Analyzers.Settings;
using Scriban;
using Scriban.Runtime;

namespace Gundi.Rendering;

internal static class TemplateRender
{
    private static readonly Lazy<Template> UnionTemplate = new(() => Template.Parse(LoadTemplateScript("Union")));

    private static string LoadTemplateScript(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var manifestStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Rendering.{name}.sbn-cs");
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