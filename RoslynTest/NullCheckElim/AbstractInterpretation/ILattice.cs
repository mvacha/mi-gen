using System;

namespace NullCheckElim
{
    public interface ILattice<TEnum> where TEnum : Enum
    {
        TEnum Value { get; }
        //Returns true if Value has changed
        bool MergeWith(TEnum newValue);
        void Update(TEnum newValue);
    }
}
