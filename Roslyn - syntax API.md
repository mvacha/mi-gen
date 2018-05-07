## Syntax/Semantics API

**Blue nodes** - Syntax nodes (broken down into other types)

**Green nodes** - Syntax tokens (cannot be broken into smaller pieces)

**White/Gray** - Trivia (not relevant for compilation - e.g. whitespace, comments...)



### Syntax nodes

Roslyn types are visible in the Syntax Visualizer.

Immutable

`With[PropertyName]` - F# like syntax for modifying special props

SyntaxFactory



### Syntax tokens

`.ToFullString()` - toString with trivia (tabs, spaces...)



### Syntax walkers

DFS walk

Visits only SyntaxNodes by default -changed via constructor arg `base(SyntaxWalkerDepth.Token)`

When visiting SyntaxNodes - call to base implementation is required (they handle the traversal deeper into the tree).



### Syntax rewriter

Builds up tree from the bottom.

`return null` - remove visited node

`return node` - return the same node, do not traverse deeper

Formater class - format syntax trees to look "human".

Replacing nodes using `.ReplaceNode()` silently breaks when called with node (to replace) from another tree (e.g. in a foreach)



### Semantic Model and Symbols

Semantic model contains information about individual symbols (e.g. classes, methods...) with information about their Assembly, namespace etc...

When program does not compile - SymbolInfo may contain multiple CandidateSymbols instead of just one Symbol

Caches info for further calls (GetDeclaredSymbols etc..) but SemanticModel itself is not cached.



### Workspaces

`MSBuildWorkspace`  - manipulating VS Solutions on disc

`VisualStudioWorkspace` - manipulating live VS solution (e.g. from a VS plugin)

`AdHocWorkspace` - testing workspace, no need to call `TryApplyChanges` to apply changes



### VS Analyters

Nuget package or vsix addon




