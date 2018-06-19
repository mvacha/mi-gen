using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NullCheckElim.App
{
    class Program
    {
        static void Main()
        {
            // RegionTest
            var source1 = @"
            class Program{
                void Main()
                {
                    {
                        int i;
                        i = 10;
                    }
                    {
                        int i;
                        i = 20;
                    }
                }
            }";

            //CompileAndPrintCFG(source1);

            //Packing Test
            var source2 = @"
            class Program{
                void Main()
                {
                    int i;
                    { i = 10; }
                    { i = 20; }
                }
            }";

            //CompileAndPrintCFG(source2);

            //Lowering Test - ternary operator
            var source3 = @"
            class Program{
                int Main()
                {
                    return true ? 1 : 0;
                }
            }";

            //CompileAndPrintCFG(source3);

            //Transformation test - if else
            var source4 = @"
            class Program{
                int Main()
                {
                    int i = 0;
                    if (true)
                        i = 10;
                    else
                        i = 20;

                    return i;
                }
            }";

            //region test
            var source5 = @"
            class Program{
                int Main()
                {
                    var str = new string('a', 10);
                    var d = DateTime.Now;
                }
            }";

            const string a = "10";
            var b = a;

            var compilation = Compile(source5);
            var cfg = GetTreeCFG(compilation);

            PrettyPrintCFG(compilation);
            TestAbstractImplementation(GetTreeCFG(compilation));

#if DEBUG
            Console.ReadKey();
#endif

        }
   
        static Compilation Compile(string code)
        {
            var parserOptions = new CSharpParseOptions(languageVersion: LanguageVersion.CSharp7);
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

        static ControlFlowGraph GetTreeCFG(Compilation compilation)
        {
            var tree = compilation.SyntaxTrees.Single();

            var root = tree.GetRoot();

            var semanticModel = compilation.GetSemanticModel(tree);
            var firstMethod = root.DescendantNodes().OfType<BaseMethodDeclarationSyntax>().First();
            var firstMethodOper = semanticModel.GetOperation(firstMethod) as IMethodBodyOperation;

            return ControlFlowGraph.Create(firstMethodOper);
        }

        static void PrettyPrintCFG(Compilation compilation)
        {
            var cfg = GetTreeCFG(compilation);
            
            //TODO: #ifdef ROSLYN-LOCAL
            var cfgPrettyPrint = ControlFlowGraphVerifier.GetFlowGraph(compilation, cfg);

            Console.WriteLine(cfgPrettyPrint);
            Console.WriteLine();
        }

        static void TestAbstractImplementation(ControlFlowGraph cfg)
        {
            var mainBB = cfg.Blocks.Single(bb => bb.Kind == BasicBlockKind.Block);
            var mainRegion = mainBB.EnclosingRegion;

            var mainEvalRegion = EvaluatedRegion<NullLattice, NullLatticeValue>.CreateFromRegion(cfg.Root.NestedRegions[0]);

            var analysis = new NullCheckElimination();
            analysis.InterpretBB(mainBB, mainEvalRegion);

            //Nested regions not yet supported
            Debug.Assert(cfg.Root.NestedRegions.Count() == 1);
        }
    }
}

    //public static async Task Main(string[] args)
    //{
    //    string path = @"C:\Users\test\Repos\Topshelf\src\TopShelf.sln";
    //    var msbws = MSBuildWorkspace.Create();
    //    var solution = await msbws.OpenSolutionAsync(path);

    //    foreach (var project in solution.Projects)
    //    {
    //        foreach (var doc in project.Documents)
    //        {
    //            var name = doc.Name;
    //            var fp = doc.FilePath;
    //        }
    //    }
    //}


    //static void Main()
    //{
    //    var tree = CSharpSyntaxTree.ParseText(@"
    //    public partial class PartialClass
    //    {
    //        void Method()
    //        {
    //            System.Console.WriteLine(""YEY!"");
    //        }
    //    }

    //    public partial class PartialClass
    //    {
    //    }
    //    ");

    //    var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
    //    var compilation = CSharpCompilation.Create("MyCompilation",
    //        syntaxTrees: new[] { tree }, references: new[] { mscorlib });

    //    var root = tree.GetRoot();
    //    var semanticModel = compilation.GetSemanticModel(tree);
    //    var methodSyntax = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();

    //    var methodSymbol = semanticModel.GetDeclaredSymbol(methodSyntax);

    //    //var assembly = methodSymbol.ContainingAssembly;

    //    //var firstClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
    //    //var secondClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Last();

    //    //var firstSymbol = semanticModel.GetDeclaredSymbol(firstClass);
    //    //var secondSymbol = semanticModel.GetDeclaredSymbol(secondClass);

    //    //Debug.Assert(firstSymbol == secondSymbol);
    //    //Debug.Assert(firstClass != secondClass);

    //    var invokedMethod = root.DescendantNodes().OfType<InvocationExpressionSyntax>().Single();
    //    var symbolInfo = semanticModel.GetSymbolInfo(invokedMethod);
    //    var invokedSymbols = symbolInfo.Symbol as IMethodSymbol;

    //    var containingAssembly = invokedSymbols.ContainingAssembly; //mscorlib

    //}
    //    static void Main()
    //    {
    //        var tree = CSharpSyntaxTree.ParseText(@"
    //            class Program{
    //                void Main()
    //                {
    //                    if (true)
    //                        Console.WriteLine(""It was true!"");
    //                    if (true)
    //                        Console.WriteLine(""Breaks ReplaceNode!"");
    //                }
    //            }
    //    ");

    //        var root = tree.GetRoot();
    //        //var rewriter = new MyRewriter();
    //        //var newRoot = rewriter.Visit(root);

    //        var ifStatements = root.DescendantNodes().OfType<IfStatementSyntax>();
    //        foreach (var ifStatement in ifStatements)
    //        {
    //            var body = ifStatement.Statement;
    //            var block = SyntaxFactory.Block(body);
    //            var newIf = ifStatement.WithStatement(block);
    //            root = root.ReplaceNode(ifStatement, newIf);
    //        }

    //    }
    //}

    //public class MyRewriter : CSharpSyntaxRewriter
    //{
    //    public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
    //    {
    //        var body = node.Statement;
    //        var block = SyntaxFactory.Block(body);
    //        var newIf = node.WithStatement(block);
    //        return newIf;
    //    }
    //}


    //        static void Main()
    //        {
    //            var tree = CSharpSyntaxTree.ParseText(@"
    //class C{
    //    void Method()
    //    {
    //    }
    //}
    //");
    //            var root = tree.GetRoot();
    //            var walker = new MyWalker();
    //            walker.Visit(root);

    //            var result = walker.sb.ToString();
    //        }
    //    }

    //    public class MyWalker : CSharpSyntaxWalker
    //    {
    //        public MyWalker() : base(SyntaxWalkerDepth.Token)
    //        {

    //        }

    //        public StringBuilder sb = new StringBuilder();
    //        public override void VisitToken(SyntaxToken token)
    //        {
    //            //sb.Append(token.ToFullString());
    //        }

    //        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    //        {
    //            sb.Append(node.ToString());
    //            base.VisitMethodDeclaration(node);
    //        }
    //    }


    //        static void Main(string[] args)
    //        {
    //            var tree = CSharpSyntaxTree.ParseText(@"
    //class C{
    //    void Method()
    //    {
    //    }
    //}
    //");
    //            var root = tree.GetRoot();
    //            var classC = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
    //            var method = classC.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
    //            var returnType = SyntaxFactory.ParseTypeName("string");
    //            var newMethod = method.WithReturnType(returnType);
