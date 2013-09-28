using System;
using System.Collections.Generic;
using com.bodurov.NdSpace.Model;

namespace com.bodurov.NdSpace.Interface
{
    public interface ISpaceManager
    {
        Space<T> CreateSpace<T>(ISpaceConfig config);
        SpacePoint<T> AddPoint<T>(Space<T> space, T data, params float[] dimensionPositions);
        bool AddPoint<T>(Dimension<T> dimension, SpacePoint<T> data, float position);
        bool Reposition<T>(Dimension<T> dimension, SpacePoint<T> point, float position);
        bool TryFindDimensionPoint<T>(Dimension<T> dimension, float position, out DimensionPoint<T> left, out DimensionPoint<T> right);
        bool TryFindFirstSpacePoint<T>(Space<T> space, out SpacePoint<T> point, params float[] dimensionPositions);
        bool TryFindSpacePoints<T>(Space<T> space, out HashSet<SpacePoint<T>> points, params float[] dimensionPositions);
        bool TryAddLevel<T>(Dimension<T> dimension);
        bool TryExtendUp<T>(DimensionLink<T> toExtendUp, out DimensionLink<T> nextUpExtension);
        bool RemovePoint<T>(SpacePoint<T> sp);
        void PopulateSpace<T>(Space<T> space, List<SpacePointSource<T>> points);
        void ClearSpace<T>(Space<T> space);
        bool TryRemoveLevel<T>(Dimension<T> dimension);
        SpacePoint<T> FindNearest<T>(SpacePoint<T> center, float within);
        /// <typeparam name="T">the type of space point value</typeparam>
        /// <param name="center">center point</param>
        /// <param name="where">(current, center, distanceBetween)</param>
        /// <param name="within">include points within</param>
        /// <returns>to include or not</returns>
        IEnumerable<SpacePoint<T>> FindAllNearWhere<T>(SpacePoint<T> center, Func<SpacePoint<T>, SpacePoint<T>, float, bool> where, float within);
        IEnumerable<SpacePoint<T>> FindAllNear<T>(SpacePoint<T> center, float within);
        /// <typeparam name="TSource">the type of space point value</typeparam>
        /// <typeparam name="TAccumulate">the type of the accumulate</typeparam>
        /// <param name="center">center point</param>
        /// <param name="seed">accumulate seed</param>
        /// <param name="func">(accumulate, current, center, distanceBetween)</param>
        /// <param name="within">include points within</param>
        /// <returns>accumulate</returns>
        TAccumulate AggregateNear<TSource, TAccumulate>(SpacePoint<TSource> center, TAccumulate seed, Func<TAccumulate, SpacePoint<TSource>, SpacePoint<TSource>, float, TAccumulate> func, float within);

        IEnumerable<PointAndDistance<T>> FindAllNearWhereWithDistance<T>(SpacePoint<T> center, Func<SpacePoint<T>, SpacePoint<T>, float, bool> where, float within);
        IEnumerable<PointAndDistance<T>> FindAllNearWithDistance<T>(SpacePoint<T> center, float within);
        bool Reposition<T>(SpacePoint<T> sp, params float[] dimensionPositions);
        bool Reposition<T>(SpacePoint<T> sp, float x, float y, float z);
    }
}