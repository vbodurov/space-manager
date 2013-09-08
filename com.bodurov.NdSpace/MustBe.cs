using System;

namespace com.bodurov.NdSpace
{
    public static class MustBe
    {
        public static void NotNull<T>(T obj, Func<string> getInfo) where T : class
        {
            if (obj == null) throw new ArgumentException("Expected NOT NULL for object of type '" + typeof(T) + "' Details:" + getInfo());
        }

        public static void Null<T>(T obj, Func<string> getInfo) where T : class
        {
            if (obj != null) throw new ArgumentException("Expected NULL for object of type type '" + typeof(T) + "' Details:" + getInfo());
        }

        public static void Equal<T>(T a, T b, Func<string> getInfo) where T : IEquatable<T>
        {
            if (!a.Equals(b)) throw new ArgumentException("Expected to be equal '" + a + "' and '" + b + "' of type '"+typeof(T)+"' Details:" + getInfo());
        }
    }
}