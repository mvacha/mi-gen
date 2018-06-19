using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullCheckElim
{
    public static class Extensions
    {
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue> (this IDictionary<TKey, TValue> dictionary)
            => new ReadOnlyDictionary<TKey, TValue>(dictionary);

        public static bool IsNullable(this ITypeSymbol type)
            => type.IsReferenceType || type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
    }
}
