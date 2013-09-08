using System;

namespace com.bodurov.NdSpace.Model
{
    public class SpacePointSource<T> : SpacePoint<T>
    {
        public SpacePointSource(Space<T> space, T value, params float[] positions) : base(space)
        {
            if (positions.Length != space.Dimensions.Length) throw new ArgumentException("SpacePointSource Expected "+space.Dimensions.Length+" dimensions found "+positions.Length);
            InitialPositions = positions;
            Value = value;
        }

        public float[] InitialPositions { get; internal set; }
    }
}