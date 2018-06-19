using Microsoft.CodeAnalysis;
using System.Linq;

namespace NullCheckElim.App
{
    public static class RoslynExtensions
    {
        public static bool HasErrors(this SyntaxNode node)
        {
            if (!node.ContainsDiagnostics) //fast path
                return false;

            return node.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error);
        }
    }
}
