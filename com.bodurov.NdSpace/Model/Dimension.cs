using System;

namespace com.bodurov.NdSpace.Model
{
    public class Dimension<T> : BaseSpaceObject
    {

        public Dimension(byte index, Space<T> space)
        {
            Index = index;
            Space = space;
            Count = 0;
            Epsillon = space == null ? 0.1f : space.Config.DefaultEpsilon;
        }

        public DimensionPoint<T> HeadDimPoint { get; internal set; }
        public DimensionPoint<T> TailDimPoint { get; internal set; }
        public Space<T> Space { get; private set; }
        public byte Index { get; private set; }
        public byte NumberLevels { get { return (byte)(Count < 2 ? Count : Math.Ceiling(Math.Log(Count, 2))); } }
        public int Count { get; internal set; }
        public double Epsillon { get; internal set; }


//        public bool Within(double c, double min, double max)
//        {
//            return (c >= (min - Epsillon) && c <= (max + Epsillon));
//        }
        public bool Eq(double a, double b)
        {
            return Math.Abs(a - b) <= Epsillon;
        }
//        public bool LowOrEq(double a, double b)
//        {
//            if (Math.Abs(a - b) <= Epsillon) return true;
//            return a < b;
//        }
//        public bool HighOrEq(double a, double b)
//        {
//            if (Math.Abs(a - b) <= Epsillon) return true;
//            return a > b;
//        }
    }
}