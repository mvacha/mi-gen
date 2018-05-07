## Getting latest roslyn:

1. Get Moniker + Version from: https://github.com/dotnet/roslyn/blob/features/dataflow/build/Targets/Versions.props 
2. Check PublishData.json: https://github.com/dotnet/roslyn/blob/master/build/config/PublishData.json
3. See if package is on public myget:
   https://dotnet.myget.org/feed/roslyn/package/nuget/ [Package name]
   e.g. https://dotnet.myget.org/feed/roslyn/package/nuget/Microsoft.CodeAnalysis.CSharp.Scripting
4. Ctrl+F for moniker/version combination in the package history (ordered by version).



```c#
protected static void VerifyFlowGraphForTest<TSyntaxNode>(CSharpCompilation compilation, string expectedFlowGraph)
    where TSyntaxNode : SyntaxNode
        {
            var tree = compilation.SyntaxTrees[0];
            var model = compilation.GetSemanticModel(tree);
            SyntaxNode syntaxNode = GetSyntaxNodeOfTypeForBinding<TSyntaxNode>(GetSyntaxNodeList(tree));

            Operations.ControlFlowGraph graph = SemanticModel.GetControlFlowGraph((Operations.IBlockOperation)model.GetOperation(syntaxNode));
            ControlFlowGraphVerifier.VerifyGraph(compilation, expectedFlowGraph, graph);
        }	
```