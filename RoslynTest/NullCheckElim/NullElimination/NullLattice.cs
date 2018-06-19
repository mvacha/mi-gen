using System.Diagnostics;

namespace NullCheckElim
{
    enum NullLatticeValue
    {
        Unknown = 0,
        NotNull = 1,
        Null = 2,
        SometimesNull = 10,
    }

    [DebuggerDisplay("{Value}")]
    class NullLattice : ILattice<NullLatticeValue>
    {
        //private NullLaticeValue _value; //initialized to Unknown (0) by default
        public NullLatticeValue Value { get; private set; }

        public bool MergeWith(NullLatticeValue newValue)
        {
            switch (newValue)
            {
                //Same value
                case var v when Value == v:
                    return false;

                //Merge with supremum when value is Unknown
                case var supremum when Value == NullLatticeValue.Unknown:
                    Value = supremum;
                    return true;

                //Merge with "neighbor" and move upwards
                case NullLatticeValue.NotNull when Value == NullLatticeValue.Null:
                case NullLatticeValue.Null when Value == NullLatticeValue.NotNull:

                //Merge with supremum and move upwards
                case NullLatticeValue.SometimesNull when Value != NullLatticeValue.SometimesNull:

                    Value = NullLatticeValue.SometimesNull;
                    return true;

                // merge with inf.
                default:
                    return false;

            }

        }

        public void Update(NullLatticeValue newValue)
            => Value = newValue;
    }
}
