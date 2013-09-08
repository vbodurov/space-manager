using System;

namespace com.bodurov.NdSpace.Extensions
{
    public static class GlobalExtensions
    {
         public static T Do<T>(this T obj, Action<T> action)
         {
             action(obj);
             return obj;
         }
    }
}