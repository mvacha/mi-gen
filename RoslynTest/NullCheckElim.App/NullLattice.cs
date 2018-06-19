using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullCheckElim.App
{
    interface ILattice<TEnum> where TEnum : Enum
    {
        TEnum Value { get; }
        //Returns true if Value has changed
        bool MergeWith(TEnum newValue);
        void Update(TEnum newValue);
    }


    enum NullLaticeValue
    {
        Unknown = 0,
        NotNull = 1,
        Null = 2,
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
                case NullLaticeValue.NotNull when Value == NullLaticeValue.Null:
                case NullLaticeValue.Null when Value == NullLaticeValue.NotNull:

                //Merge with supremum and move upwards
                case NullLaticeValue.SometimesNull when Value != NullLaticeValue.SometimesNull:

                    _value = NullLaticeValue.SometimesNull;
                    return true;

                // merge with inf.
                default:
                    return false;

            }

        }

        public void Update(NullLaticeValue newValue)
            => _value = newValue;
    }
}
