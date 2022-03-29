using Microsoft.CodeAnalysis;

namespace Gundi;

internal static class Inspect
{
    public static IEnumerable<Diagnostic> TypeHasConstructorWithTwoStringsAndType(this INamedTypeSymbol type)
    {
        if (type.Constructors.Any(x => x.Parameters.Length == 3 &&
                                       x.Parameters[0].Type.Name == nameof(Type) &&
                                       x.Parameters[1].Type.Name == nameof(String) &&
                                       x.Parameters[2].Type.Name == nameof(String)))
        {
            yield return Diagnose.Error(
                nameof(TypeHasConstructorWithTwoStringsAndType),
                "Type missing constructor",
                $"Type '{type.ToDisplayString()}' must have a constructor of params `(Type, String, String)`.",
                Location.None
            );
        }
    }
}