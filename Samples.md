#### Definite assignment

Following code is invalid in C#:

```c#
string abc;
if (abc == null)
```

Variable of a (nullable) reference type always has to be assigned before it's value is used.





#### CFG of lowered conditional expression

```c#
{ return true ? 1 : 0; }
```



Gets lowered to:

```
Block[B0] - Entry
    Statements (0)
    Next (Regular) Block[B1]
Block[B1] - Block
    Predecessors: [B0]
    Statements (0)
    Jump if False (Regular) to Block[B3]
        ILiteralOperation (OperationKind.Literal, Type: System.Boolean, Constant: True) (Syntax: 'true')

    Next (Regular) Block[B2]
Block[B2] - Block
    Predecessors: [B1]
    Statements (1)
        IFlowCaptureOperation: 0 (OperationKind.FlowCapture, Type: null, IsImplicit) (Syntax: '1')
          Value:
            ILiteralOperation (OperationKind.Literal, Type: System.Int32, Constant: 1) (Syntax: '1')

    Next (Regular) Block[B4]
Block[B3] - Block [UnReachable]
    Predecessors: [B1]
    Statements (1)
        IFlowCaptureOperation: 0 (OperationKind.FlowCapture, Type: null, IsImplicit) (Syntax: '0')
          Value:
            ILiteralOperation (OperationKind.Literal, Type: System.Int32, Constant: 0) (Syntax: '0')

    Next (Regular) Block[B4]
Block[B4] - Block
    Predecessors: [B2] [B3]
    Statements (0)
    Next (Return) Block[B5]
        IFlowCaptureReferenceOperation: 0 (OperationKind.FlowCaptureReference, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: 'true ? 1 : 0')
Block[B5] - Exit
    Predecessors: [B4]
    Statements (0)
```



`IFlowCaptureReferenceOperation` has link `Syntax` to `SyntaxNode` representing the original condition (`true ? 1 : 0` ). 

```
cfg.Blocks[4].Next.Value.Syntax.ToFullString()
```

Returns:

```
"true ? 1 : 0"
```

 And it's parent is the return statement and then the enclosing block statement:

```
cfg.Blocks[4].Next.Value.Syntax.Parent.Parent.ToString();
"{\r\n                    return true ? 1 : 0;\r\n                }"
```



#### If-else statement

```c#
{
    int i = 0;
    if (true)
   		i = 10;
    else
    	i = 20;

    return i;
}
```

Gets trto:

```
Block[B0] - Entry
    Statements (0)
    Next (Regular) Block[B1]
        Entering: {R1}

.locals {R1}
{
    Locals: [System.Int32 i]
    Block[B1] - Block
        Predecessors: [B0]
        Statements (1)
            ISimpleAssignmentOperation (OperationKind.SimpleAssignment, Type: System.Int32, IsImplicit) (Syntax: 'i = 0')
              Left:
                ILocalReferenceOperation: i (IsDeclaration: True) (OperationKind.LocalReference, Type: System.Int32, IsImplicit) (Syntax: 'i = 0')
              Right:
                ILiteralOperation (OperationKind.Literal, Type: System.Int32, Constant: 0) (Syntax: '0')

        Jump if False (Regular) to Block[B3]
            ILiteralOperation (OperationKind.Literal, Type: System.Boolean, Constant: True) (Syntax: 'true')

        Next (Regular) Block[B2]
    Block[B2] - Block
        Predecessors: [B1]
        Statements (1)
            IExpressionStatementOperation (OperationKind.ExpressionStatement, Type: null) (Syntax: 'i = 10;')
              Expression:
                ISimpleAssignmentOperation (OperationKind.SimpleAssignment, Type: System.Int32) (Syntax: 'i = 10')
                  Left:
                    ILocalReferenceOperation: i (OperationKind.LocalReference, Type: System.Int32) (Syntax: 'i')
                  Right:
                    ILiteralOperation (OperationKind.Literal, Type: System.Int32, Constant: 10) (Syntax: '10')

        Next (Regular) Block[B4]
    Block[B3] - Block [UnReachable]
		...
		
    Block[B4] - Block
        Predecessors: [B2] [B3]
        Statements (0)
        Next (Return) Block[B5]
            ILocalReferenceOperation: i (OperationKind.LocalReference, Type: System.Int32) (Syntax: 'i')
            Leaving: {R1}
}

Block[B5] - Exit
    Predecessors: [B4]
    Statements (0)
```

Original if statement is reachable via:

```
Blocks[1].Conditional.Condition.Syntax.Parent
```

Which returns: `"if (true) i = 10; else i = 20;"` 

