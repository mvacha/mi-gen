using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullCheckElim
{
    public abstract class BaseOperationsInterpreter<TLattice, TEnum> : OperationWalker
        where TLattice : ILattice<TEnum>, new()
        where TEnum : Enum
    {
        public EvaluatedRegion<TLattice, TEnum> CurrentEval { get; }
        public bool HasChanged { get; }

        protected BaseOperationsInterpreter(EvaluatedRegion<TLattice, TEnum> startEval)
        {
            CurrentEval = startEval;
        }
    }
}
