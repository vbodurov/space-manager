using System;

namespace com.bodurov.NdSpace.Extensions
{
    public static class SingleExtensions
    {
        public const float Epsillon = 0.0000001f;

        public static bool IsEqualTo(this float a, float b, float epsillon = Epsillon)
        {
            return Math.Abs(a - b) < epsillon;
        }
        public static float Abs(this float n)
        {
            return n < 0 ? n*-1 : n;
        }
    }
}