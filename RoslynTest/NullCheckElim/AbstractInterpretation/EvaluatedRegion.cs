using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullCheckElim
{
    public class EvaluatedRegion<TLattice, TEnum>
        where TLattice : ILattice<TEnum>, new()
        where TEnum : Enum
    {
        //TODO: Handle nested regions
        //public EvaluatedRegion<TLattice, TEnum> EnclosingRegion;

        public IReadOnlyDictionary<ILocalSymbol, TLattice> Symbols { get; }

        //TODO: Extend indexer to handle nested regions
        public TLattice this[ILocalSymbol symbol] => Symbols[symbol];

        public EvaluatedRegion(IReadOnlyDictionary<ILocalSymbol, TLattice> symbols)
        {
            Symbols = symbols;
        }

        public static EvaluatedRegion<TLattice, TEnum> CreateFromRegion(ControlFlowRegion region)
        {
            var symbols = region.Locals
                //.Where(l => !l.IsConst && !)
                .ToDictionary(l => l, l => new TLattice())
                .AsReadOnly();

            return new EvaluatedRegion<TLattice, TEnum>(symbols);
        }
    }
}
