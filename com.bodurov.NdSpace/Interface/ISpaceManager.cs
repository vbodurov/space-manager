using System.Collections.Generic;
using com.bodurov.NdSpace.Model;

namespace com.bodurov.NdSpace.Interface
{
    public interface ISpaceManager
    {
        Space<T> CreateSpace<T>(ISpaceConfig config = null);
        SpacePoint<T> AddPoint<T>(Space<T> space, T data, params float[] dimensionPositions);
        bool AddPoint<T>(Dimension<T> dimension, SpacePoint<T> data, float position);
        bool Reposition<T>(Dimension<T> dimension, SpacePoint<T> point, float position);
        bool TryFindPoint<T>(Dimension<T> dimension, float position, out DimensionPoint<T> left, out DimensionPoint<T> right);
        bool TryAddLevel<T>(Dimension<T> dimension);
        bool TryExtendUp<T>(DimensionLink<T> toExtendUp, out DimensionLink<T> nextUpExtension);
        bool RemovePoint<T>(SpacePoint<T> sp);
        void PopulateSpace<T>(Space<T> space, List<SpacePointSource<T>> points);
        void ClearSpace<T>(Space<T> space);
        bool TryRemoveLevel<T>(Dimension<T> dimension);
    }
}