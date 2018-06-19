using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NullCheckElim.Tests
{
    public static class RoslynHelpers
    {
        public static Compilation Compile(string code)
        {
            var parserOptions = new CSharpParseOptions(languageVersion: LanguageVersion.CSharp7_3);
            var parsedTree = CSharpSyntaxTree.ParseText(code, options: parserOptions);

            //Assert parser success
            Debug.Assert(!parsedTree.GetRoot().HasErrors());

            //Enable flow-analysis feature flag
            var options = parsedTree.Options.WithFeatures(new[] { new KeyValuePair<string, string>("flow-analysis", "true") });
            parsedTree = parsedTree.WithRootAndOptions(parsedTree.GetRoot(), options);

            //Compile and link with. mscorlib.dll
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { parsedTree },
                references: new[] { mscorlib });

            return compilation;
        }

        public static ControlFlowGraph GetTreeCFG(this Compilation compilation)
        {
            var tree = compilation.SyntaxTrees.Single();

            var root = tree.GetRoot();

            var semanticModel = compilation.GetSemanticModel(tree);
            var firstMethod = root.DescendantNodes().OfType<BaseMethodDeclarationSyntax>().First();
            var firstMethodOper = semanticModel.GetOperation(firstMethod) as IMethodBodyOperation;


            return SemanticModel.GetControlFlowGraph(firstMethodOper);
        }

        public static ControlFlowGraph CompileGetCFG(string code)
            => Compile(code).GetTreeCFG();

        public static string WrapInMain(string str)
            => $"class Program{{ int Main(){{ {str} }} }}";

        public static bool HasErrors(this SyntaxNode node)
        {
            if (!node.ContainsDiagnostics) //fast path
                return false;

            return node.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error);
        }

        internal static Dictionary<string, NullLatticeValue> ToSearchableDictionary(this IReadOnlyDictionary<ILocalSymbol, NullLattice> origDict)
            => origDict.ToDictionary(l => l.Key.ToString(), l => l.Value.Value);
    }
}
