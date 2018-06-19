using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullCheckElim
{
    //Interprets operations inside BasicBlocks
    class NullOperationsInterpreter : OperationVisitor
    {
        public EvaluatedRegion<NullLattice, NullLaticeValue> CurrentEval { get; }
        public bool HasChanged { get; }

        public NullOperationsInterpreter(EvaluatedRegion<NullLattice, NullLaticeValue> startEval)
        {
            CurrentEval = startEval;
        }

        public override void DefaultVisit(IOperation operation)
        {
            Console.WriteLine($"Visiting: {operation.GetType()} -> NOOP");
        }
        

    }
}
