using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullCheckElim
{
    //Interprets operations inside BasicBlocks
    class NullOperationsInterpreter : BaseOperationsInterpreter<NullLattice, NullLatticeValue>
    {
        public NullOperationsInterpreter(EvaluatedRegion<NullLattice, NullLatticeValue> startEval) : base(startEval)
        {
        }

        //public override void DefaultVisit(IOperation operation)
        //{
        //    base.DefaultVisit(operation);
        //    Console.WriteLine($"Visiting: {operation.GetType()} -> NOOP");
        //}

        //public override void VisitExpressionStatement(IExpressionStatementOperation operation)
        //{
        //    base.VisitExpressionStatement(operation);
        //}

        public override void VisitVariableDeclaration(IVariableDeclarationOperation operation)
        {
            base.VisitVariableDeclaration(operation);
        }

        public override void VisitVariableDeclarationGroup(IVariableDeclarationGroupOperation operation)
        {
            base.VisitVariableDeclarationGroup(operation);
            throw new NotImplementedException();
        }

        public override void VisitVariableDeclarator(IVariableDeclaratorOperation operation)
        {
            base.VisitVariableDeclarator(operation);
            throw new NotImplementedException();
        }

        public override void VisitDeclarationExpression(IDeclarationExpressionOperation operation)
        {
            base.VisitDeclarationExpression(operation);
            throw new NotImplementedException();
        }

        public override void VisitSimpleAssignment(ISimpleAssignmentOperation operation)
        {
            base.VisitSimpleAssignment(operation);

            //TODO: support other than local references
            var target = operation.Target as ILocalReferenceOperation;

            if (!target.Type.IsNullable())
                return;

            var targetEval = CurrentEval[target.Local];
            var value = operation.Value;

            if (value is IConversionOperation conversion)
            {
                value = conversion.Operand;
            }


            switch (value)
            {
                case ILiteralOperation _ when value.ConstantValue.HasValue && value.ConstantValue.Value == null: //null value
                case IDefaultValueOperation _: //default value
                    targetEval.Update(NullLatticeValue.Null);
                    break;

                case ILiteralOperation literal:
                case IObjectCreationOperation constructorCall:
                    targetEval.Update(NullLatticeValue.NotNull);
                    break;

                case ILocalReferenceOperation localRef:
                    var sourceRef = CurrentEval[localRef.Local];
                    targetEval.Update(sourceRef.Value);
                    break;
                case IFieldReferenceOperation fieldRef when fieldRef.Field.ContainingType.Name == "String" && fieldRef.Field.Name == "Empty":
                    targetEval.Update(NullLatticeValue.NotNull);
                    break;
                default:
                    throw new NotImplementedException();
            }


        }

        public override void VisitCompoundAssignment(ICompoundAssignmentOperation operation)
        {
            base.VisitCompoundAssignment(operation);
            throw new NotImplementedException();
        }

        //TODO: declarations and assignments in pattern matching cases
    }
}
