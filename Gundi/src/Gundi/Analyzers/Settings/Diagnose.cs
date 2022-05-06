using Microsoft.CodeAnalysis;

namespace Gundi.Analyzers.Settings;

public static class Diagnose
{
    public static IEnumerable<Diagnostic> TypeHasConstructorWithTwoStringsAndType(this INamedTypeSymbol type)
    {
        if (type.HasConstructorOfTypes(typeof(Type), typeof(string), typeof(string)) == false)
        {
            yield return DiagnoseFactory.Error(
                nameof(TypeHasConstructorWithTwoStringsAndType),
                "Type missing constructor",
                $"Type '{type.ToDisplayString()}' must have a constructor of params `(Type, String, String)`.",
                Location.None
            );
        }
    }
    
    public static IEnumerable<Diagnostic> InheritsExceptionType(this ITypeSymbol type)
    {
        bool InheritExceptionType(ITypeSymbol s)
        {
            return s.BaseType is not null &&
                   (s.BaseType.Name == nameof(Exception) || InheritExceptionType(s.BaseType));
        }
        
        if (InheritExceptionType(type) == false)
        {
            yield return DiagnoseFactory.Error(
                nameof(InheritsExceptionType),
                "Type does not inherit from Exception",
                $"Type '{type.ToDisplayString()}' must inherit from Exception.",
                Location.None
            );
        }
    }
}