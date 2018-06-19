using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullCheckElim.App
{
    public static class Extensions
    {
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue> (this IDictionary<TKey, TValue> dictionary)
            => new ReadOnlyDictionary<TKey, TValue>(dictionary);
    }
}
