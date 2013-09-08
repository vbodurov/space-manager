using System;
using System.Collections.Generic;

namespace com.bodurov.NdSpace.Extensions
{
    public static class EnumerableExtensions
    {
         public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> func)
         {
             var list = collection as IList<T>;
             if (list != null)
             {
                 for (var i = 0; i < list.Count; ++i)
                 {
                     func(list[i]);
                 }
                 return collection;
             }
// ReSharper disable PossibleMultipleEnumeration
             var enumerator = collection.GetEnumerator();
             while (enumerator.MoveNext())
             {
                 func(enumerator.Current);
             }
             return collection;
// ReSharper restore PossibleMultipleEnumeration
         }
         public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T,int> func)
         {
             var list = collection as IList<T>;
             var i = 0;
             if (list != null)
             {
                 for (; i < list.Count; ++i)
                 {
                     func(list[i], i);
                 }
                 return collection;
             }
             // ReSharper disable PossibleMultipleEnumeration
             var enumerator = collection.GetEnumerator();
             while (enumerator.MoveNext())
             {
                 func(enumerator.Current, i++);
             }
             return collection;
             // ReSharper restore PossibleMultipleEnumeration
         }
    }
}