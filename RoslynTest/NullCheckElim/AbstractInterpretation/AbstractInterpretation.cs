using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullCheckElim
{
    class NullCheckElimination : AbstractInterpretation<NullOperationsInterpreter, NullLattice, NullLatticeValue>
    {
        public NullCheckElimination() : base (t => new NullOperationsInterpreter(t))
        {

        }
    }

    //Browses basic blocks in order determined by the stack
    abstract class AbstractInterpretation<TOperationInterpreter, TLattice, TEnum>
        where TOperationInterpreter : BaseOperationsInterpreter<TLattice, TEnum>
        where TLattice : ILattice<TEnum>, new()
        where TEnum : Enum
    {
        protected IReadOnlyDictionary<int, EvaluatedRegion<TLattice, TEnum>> LastEvaluations;

        protected Queue<(BasicBlock block, EvaluatedRegion<TLattice, TEnum> eval)> TodoQueue { get; }

        protected Func<EvaluatedRegion<TLattice, TEnum>, TOperationInterpreter> InterpreterFactory { get; }

        public AbstractInterpretation(Func<EvaluatedRegion<TLattice, TEnum>, TOperationInterpreter> intepreterFactory)
        {
            TodoQueue = new Queue<(BasicBlock block, EvaluatedRegion<TLattice, TEnum> eval)>();
            InterpreterFactory = intepreterFactory;
        }

        public EvaluatedRegion<TLattice, TEnum> InterpretBB(BasicBlock block, EvaluatedRegion<TLattice, TEnum> reg)
        {
            var interpreter = InterpreterFactory(reg);
            foreach (var operation in block.Operations)
            {
                interpreter.Visit(operation);
            }

            //FIXME: duplicate evaluated region for comparation
            return reg;
        }

        public void Interpret(ControlFlowGraph cfg)
        {
            LastEvaluations = cfg.Blocks
                //TODO: filter blocks without more than one predecessor
                .ToDictionary(keySelector: bb => bb.Ordinal,
                              elementSelector: bb => EvaluatedRegion<TLattice, TEnum>.CreateFromRegion(bb.EnclosingRegion))
                .AsReadOnly();

            TodoQueue.Enqueue((cfg.Blocks[0], LastEvaluations[0]));

            while (TodoQueue.Any())
            {
                (var workingBlock, var initEval) = TodoQueue.Dequeue();

                InterpretBB(workingBlock, initEval);

                //interpretation result handling

                if (workingBlock.Kind != BasicBlockKind.Exit)
                {
                    //Handle Next
                    //var dest = workingBlock.Next.Branch.Destination;
                    //var destEval = LastEvaluations[dest.Ordinal];
                    
                    //TODO: branching kinds
                    

                    //Handle Condition
                }
            }

        }

    }
}
