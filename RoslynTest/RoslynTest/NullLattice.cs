using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynTest
{
    interface ILattice<TEnum> where TEnum : struct
    {
        TEnum Value { get; }

        //Returns true if Value has changed
        bool MergeWith(TEnum newValue);
    }


    enum NullLaticeValue
    {
        Unknown = 0,
        NeverNull = 1,
        AlwaysNull = 2,
        SometimesNull = 10,
    }

    struct NullLattice : ILattice<NullLaticeValue>
    {
        private NullLaticeValue _value; //initialized to Unknown (0) by default
        public NullLaticeValue Value => _value;

        public bool MergeWith(NullLaticeValue newValue)
        {
            switch (newValue)
            {
                //Same value
                case var v when _value == v:
                    return false;

                //Merge with supremum when value is Unknown
                case var supremum when _value == NullLaticeValue.Unknown:
                    _value = supremum;
                    return true;

                //Merge with "neighbor" and move upwards
                case NullLaticeValue.NeverNull when Value == NullLaticeValue.AlwaysNull:
                case NullLaticeValue.AlwaysNull when Value == NullLaticeValue.NeverNull:

                //Merge with supremum and move upwards
                case NullLaticeValue.SometimesNull when Value != NullLaticeValue.SometimesNull:

                    _value = NullLaticeValue.SometimesNull;
                    return true;

                // merge with inf.
                default:
                    return false;

            }

        }
    }
}
