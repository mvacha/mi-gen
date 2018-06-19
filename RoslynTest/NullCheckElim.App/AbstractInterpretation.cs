using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullCheckElim.App
{
    //Browses basic blocks in order determined by the stack
    class AbstractInterpretation
    {

        protected IReadOnlyDictionary<int, EvaluatedRegion<NullLattice, NullLaticeValue>> LastEvaluations;

        protected Queue<(BasicBlock block, EvaluatedRegion<NullLattice, NullLaticeValue> eval)> TodoQueue { get; }

        public AbstractInterpretation()
        {
            TodoQueue = new Queue<(BasicBlock block, EvaluatedRegion<NullLattice, NullLaticeValue> eval)>();
        }

        public void Interpret(ControlFlowGraph cfg)
        {
            LastEvaluations = cfg.Blocks
                //TODO: filter blocks without more than one predecessor
                .ToDictionary(keySelector: bb => bb.Ordinal,
                              elementSelector: bb => EvaluatedRegion<NullLattice, NullLaticeValue>.CreateFromRegion(bb.EnclosingRegion))
                .AsReadOnly();

            TodoQueue.Enqueue((cfg.Blocks[0], LastEvaluations[0]));

            while (TodoQueue.Any())
            {
                (var workingBlock, var initEval) = TodoQueue.Dequeue();

                var interpretation = new NullOperationsInterpreter(initEval);
                foreach (var operation in workingBlock.Operations)
                {
                    interpretation.Visit(operation);
                }

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
