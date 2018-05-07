## Using Local Build of (some) Roslyn libraries

Some libs are not shipped as nuget packages - e.g. `Roslyn.Test.Utilities.dll`, but they contain usefull classes for debugging and testing code which uses IOperations and CFGs. For example `OperationTreeVerifier.GetOperationTree` pretty-prints the whole IOperations tree.

1. Reference the required library directly from the build output of Roslyn `\roslyn\Binaries\Debug\Dlls\TestUtilities\net46\Roslyn.Test.Utilities.dll`.

Localy built roslyn assemblies have their version changed to **42.42.42.42**.

1. Add references (again from roslyn directory) for all dependencies that are causing conflict between libraries referenced from the nuget packages and the same libraries (but with version 42.42.42.42) referenced by the localy-built roslyn library.

2. Add BindingRedirect for conflicting libraries (resolve conflict between versions by redirecting to version 42.42.42.42)

   ```xml
   <dependentAssembly>
   	<assemblyIdentity name="Microsoft.CodeAnalysis.CSharp" publicKeyToken="31bf3856ad364e35" culture="neutral" />
   	<bindingRedirect oldVersion="0.0.0.0-42.42.42.42" newVersion="42.42.42.42" />
   </dependentAssembly>
   ```

3. Project should build without any errors now.

4. We can call the `OperationsTreeVerifier`

   ```c#
   var operStr = OperationTreeVerifier.GetOperationTree(compilation, oper);
   ```

   `operStr` now contains the pretty-printed tree:

   ```
   IBlockOperation (2 statements) (OperationKind.Block, Type: null) ...
     IBlockOperation (2 statements, 1 locals) (OperationKind.Block, Type: null) ...
       Locals: Local_1: System.Int32 i
       IVariableDeclarationGroupOperation (1 declarations) ...
         IVariableDeclarationOperation (1 declarators) ...
           Declarators:
    ....
   ```

   