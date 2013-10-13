using System;
using com.bodurov.NdSpace.Interface;

namespace com.bodurov.NdSpace.Model
{
    public class SpacePoint<T> : BaseSpaceObject
    {

        public SpacePoint(Space<T> space)
        {
            Space = space;
            Dimensions = new DimensionPoint<T>[space.Dimensions.Length];
        }

        public Space<T> Space { get; private set; }
        public DimensionPoint<T>[] Dimensions { get; private set; }
        public T Value { get; set; }
        public bool IsFastMover { get; set; }
        public SpacePoint<T> SetFastMover()
        {
            IsFastMover = true;
            return this;
        }

        public float DistanceTo(SpacePoint<T> otherPoint)
        {
            return DistanceTo(otherPoint, null);
        }
        public float DistanceTo(SpacePoint<T> otherPoint, IDimensionSelector dimSelector)
        {
            var hasSelector = dimSelector != null;

            var sum = 0f;
            for (var d = 0; d < Dimensions.Length; ++d)
            {
                if (hasSelector && !dimSelector.IncludesDimension(d)) continue;

                var valOther = otherPoint.Dimensions[d].Position;
                var valCurrD = Dimensions[d].Position;

                var n = valCurrD - valOther;
                n = n*n;
                sum += n;
            }
            return (float)Math.Sqrt(sum);
        }

        public bool IsLocatedAt(params float[] dimensionPositions)
        {
            if (Dimensions.Length != dimensionPositions.Length)
                throw new ArgumentException("Space dimension number of (" + Dimensions.Length + ") does not much passed coordinates number of (" + dimensionPositions.Length+")");

            for (var d = 0; d < Dimensions.Length; ++d)
            {
                var dp = Dimensions[d];
                if (!dp.Dimension.Eq(dp.Position, dimensionPositions[d]))
                    return false;
            }
            return true;
        }
    }
}