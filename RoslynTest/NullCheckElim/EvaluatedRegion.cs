using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullCheckElim
{
    class EvaluatedRegion<TLattice, TENum>
        where TLattice : ILattice<TENum>, new()
        where TENum : Enum
    {
        //TODO: Handle nested regions

        public IReadOnlyList<(ILocalSymbol symbol, TLattice value)> Symbols { get; }

        private EvaluatedRegion(IReadOnlyList<(ILocalSymbol symbol, TLattice value)> symbols)
        {
            Symbols = symbols;
        }

        public static EvaluatedRegion<TLattice, TENum> CreateFromRegion(ControlFlowRegion region)
        {

            var symbols = region.Locals
                .Where(l => !l.IsFunctionValue) //TODO: maybe filter even more
                .Select(l => (l, new TLattice()))
                .ToList().AsReadOnly();

            return new EvaluatedRegion<TLattice, TENum>(symbols);
        }
    }
}
