using System;
using System.Collections.Generic;
using System.Linq;
using com.bodurov.NdSpace.Extensions;
using com.bodurov.NdSpace.Interface;
using com.bodurov.NdSpace.Model;

namespace com.bodurov.NdSpace
{
    public class SpaceManager : ISpaceManager
    {
        private readonly ISpaceManager _spaceManager;


        internal SpaceManager()
        {
            _spaceManager = this;
        }

        SpacePoint<T> ISpaceManager.FindNearest<T>(SpacePoint<T> center, float within)
        {
            var pan = new PointAndDistance<T>(null, Single.MaxValue);
            return _spaceManager.AggregateNear(center, pan, (aggr, curr, cen, dis) =>
                    {
                        if (dis < aggr.Distance)
                        {
                            aggr.Point = curr;
                            aggr.Distance = dis;
                        }
                        return aggr;
                    }, within).Point;
        }
        IEnumerable<SpacePoint<T>> ISpaceManager.FindAllNear<T>(SpacePoint<T> center, float within)
        {
            return _spaceManager.FindAllNearWhere(center, (curr, cen, dis) => true, within);
        }
        TAccumulate ISpaceManager.AggregateNear<TSource, TAccumulate>(SpacePoint<TSource> center, TAccumulate seed, Func<TAccumulate, SpacePoint<TSource>, SpacePoint<TSource>, float, TAccumulate> func, float within)
        {
            const int dimension = 0;
            var centerDimPoint = center.Dimensions[dimension];
            var centerLink = centerDimPoint.HeadLink;
            if (centerDimPoint.NumberPoints > 1)
            {
                foreach (var sp in centerDimPoint.SpaPoints.Where(p => p.ID != center.ID))
                {
                    var dictance = sp.DistanceTo(center);
                    if (dictance <= within)
                    {
                        seed = func(seed, sp, center, dictance);
                    }
                }
            }
            var centerPos = centerDimPoint.Position;
            var next = centerLink.Next;
            while (next != null && (next.Position - centerPos).Abs() <= within)
            {
                foreach(var nextSpaPoint in next.DimPoint.SpaPoints)
                {
                    var dictance = nextSpaPoint.DistanceTo(center);
                    if (dictance <= within)
                    {
                        seed = func(seed, nextSpaPoint, center, dictance);
                    }
                }
                next = next.Next;
            }
            var prev = centerLink.Prev;
            while (prev != null && (prev.Position - centerPos).Abs() <= within)
            {
                foreach(var prevSpaPoint in prev.DimPoint.SpaPoints)
                {
                    var dictance = prevSpaPoint.DistanceTo(center);
                    if (dictance <= within)
                    {
                        seed = func(seed, prevSpaPoint, center, dictance);
                    }
                }
                prev = prev.Prev;
            }
            return seed;
        }
        IEnumerable<SpacePoint<T>> ISpaceManager.FindAllNearWhere<T>(SpacePoint<T> center, Func<SpacePoint<T>, SpacePoint<T>, float, bool> where, float within)
        {
            const int dimension = 0;
            var centerDimPoint = center.Dimensions[dimension];
            var centerLink = centerDimPoint.HeadLink;
            if (centerDimPoint.NumberPoints > 1)
            {
                foreach (var sp in centerDimPoint.SpaPoints.Where(p => p.ID != center.ID))
                {
                    var dictance = sp.DistanceTo(center);
                    if (dictance <= within && where(sp, center, dictance))
                    {
                        yield return sp;
                    }
                }
            }
            var centerPos = centerDimPoint.Position;
            var next = centerLink.Next;
            while (next != null && (next.Position - centerPos).Abs() <= within)
            {
                foreach (var nextSpaPoint in next.DimPoint.SpaPoints)
                {
                    var dictance = nextSpaPoint.DistanceTo(center);
                    if (dictance <= within && where(nextSpaPoint, center, dictance))
                    {
                        yield return nextSpaPoint;
                    }
                }
                next = next.Next;
            }
            var prev = centerLink.Prev;
            while (prev != null && (prev.Position - centerPos).Abs() <= within)
            {
                foreach (var prevSpaPoint in prev.DimPoint.SpaPoints)
                {
                    var dictance = prevSpaPoint.DistanceTo(center);
                    if (dictance <= within && where(prevSpaPoint, center, dictance))
                    {
                        yield return prevSpaPoint;
                    }
                }
                prev = prev.Prev;
            }
        }
        IEnumerable<PointAndDistance<T>> ISpaceManager.FindAllNearWithDistance<T>(SpacePoint<T> center, float within)
        {
            return _spaceManager.FindAllNearWhereWithDistance(center, (curr, cen, dis) => true, within);
        }
        IEnumerable<PointAndDistance<T>> ISpaceManager.FindAllNearWhereWithDistance<T>(SpacePoint<T> center, Func<SpacePoint<T>, SpacePoint<T>, float, bool> where, float within)
        {
            const int dimension = 0;
            var centerDimPoint = center.Dimensions[dimension];
            var centerLink = centerDimPoint.HeadLink;
            if (centerDimPoint.NumberPoints > 1)
            {
                foreach (var sp in centerDimPoint.SpaPoints.Where(p => p.ID != center.ID))
                {
                    var dictance = sp.DistanceTo(center);
                    if (dictance <= within && where(sp, center, dictance))
                    {
                        yield return new PointAndDistance<T>(sp, dictance);
                    }
                }
            }
            var centerPos = centerDimPoint.Position;
            var next = centerLink.Next;
            while (next != null && (next.Position - centerPos).Abs() <= within)
            {
                foreach (var nextSpaPoint in next.DimPoint.SpaPoints)
                {
                    var dictance = nextSpaPoint.DistanceTo(center);
                    if (dictance <= within && where(nextSpaPoint, center, dictance))
                    {
                        yield return new PointAndDistance<T>(nextSpaPoint, dictance);
                    }
                }
                next = next.Next;
            }
            var prev = centerLink.Prev;
            while (prev != null && (prev.Position - centerPos).Abs() <= within)
            {
                foreach (var prevSpaPoint in prev.DimPoint.SpaPoints)
                {
                    var dictance = prevSpaPoint.DistanceTo(center);
                    if (dictance <= within && where(prevSpaPoint, center, dictance))
                    {
                        yield return new PointAndDistance<T>(prevSpaPoint, dictance);
                    }
                }
                prev = prev.Prev;
            }
        }


        Space<T> ISpaceManager.CreateSpace<T>(ISpaceConfig config)
        {
            var space = new Space<T>(config, this);

            for (byte i = 0; i < space.Dimensions.Length; ++i)
            {
                space.Dimensions[i] = new Dimension<T>(i, space);
            }

            return space;
        }

        private static bool RemovePointFromDimensions<T>(SpacePoint<T> sp, int index)
        {
            var isRemoved = false;
            var dPoint = sp.Dimensions[index];
            if (dPoint == null)
            {
                return false;
            }
            if (dPoint.NumberPoints > 1)
            {
                if (dPoint.RemovePoint(sp))
                {
                    isRemoved = true;
                }
            }
            else
            {
                if (dPoint.RemovePoint(sp))
                {
                    if (RemoveDimensionPoint(dPoint))
                    {
                        isRemoved = true;
                    }
                }
            }

            sp.Dimensions[index] = null;

            return isRemoved;
        }

        bool ISpaceManager.RemovePoint<T>(SpacePoint<T> sp)
        {
            var isRemoved = true;
            for (var i = 0; i < sp.Dimensions.Length; ++i)
            {
                if (!RemovePointFromDimensions(sp, i)) isRemoved = false;
            }
            return isRemoved;
        }

        private static bool RemoveDimensionPoint<T>(DimensionPoint<T> dPoint)
        {
            var dimension = dPoint.Dimension;
            if (dimension == null || dimension.HeadDimPoint == null)
            {
                throw new InvalidOperationException("Invalid tree: point was requested to be removed but the tree is already empty");
            }
            if (dimension.Count == 1)
            {
                if (dimension.HeadDimPoint == dPoint)
                {
                    dimension.Count = 0;
                    dimension.HeadDimPoint = dimension.TailDimPoint = null;
                    return true;
                }
                throw new InvalidOperationException("Invalid tree: point requested to be removed does not seem to belong to the tree");
            }
            if (dimension.HeadDimPoint == dPoint)
            {
                RemoveHead(dPoint.Dimension);
            }
            else if (dimension.TailDimPoint == dPoint)
            {
                RemoveTail(dPoint.Dimension);
            }
            else
            {
                RemoveMiddlePoint(dPoint);
            }
            dimension.Count--;
            return true;
        }
        private static void RemoveHead<T>(Dimension<T> dimension)
        {
            var oldHead = dimension.HeadDimPoint;
            var oldHeadLevel0 = oldHead.HeadLink;
            var newHead = oldHeadLevel0.Next.DimPoint;
            ProcessTwoColumns(oldHead.HeadLink, newHead.HeadLink, (a, b) => a.AssignNext(b.Next));


            newHead.HeadLink = oldHead.HeadLink;
            newHead.TailLink = oldHead.TailLink;
            dimension.HeadDimPoint = newHead;

            if (dimension.HeadDimPoint.HeadLink.Next != null) RemoveOrMoveColumn(dimension.HeadDimPoint.HeadLink.Next.DimPoint);
            TryRemoveLevelInternal(dimension);
        }

        

        private static void RemoveTail<T>(Dimension<T> dimension)
        {
            var oldTail = dimension.TailDimPoint;
            var oldTailLevel0 = oldTail.HeadLink;
            var newTail = oldTailLevel0.Prev.DimPoint;

            ProcessTwoColumns(newTail.HeadLink, oldTail.HeadLink, (a, b) => b.AssignPrev(a.Prev));

            newTail.HeadLink = oldTail.HeadLink;
            newTail.TailLink = oldTail.TailLink;
            dimension.TailDimPoint = newTail;

            if (dimension.TailDimPoint.HeadLink.Prev != null) RemoveOrMoveColumn(dimension.TailDimPoint.HeadLink.Prev.DimPoint);
            TryRemoveLevelInternal(dimension);
        }
        private static void RemoveMiddlePoint<T>(DimensionPoint<T> dPoint)
        {
            var link = dPoint.HeadLink;

            RemoveOrMoveColumn(dPoint);

            var prev = link.Prev;
            var next = link.Next;

            prev.AssignNext(next);
            TryRemoveLevelInternal(dPoint.Dimension);
        }

        
        private static bool RemoveOrMoveColumn<T>(DimensionPoint<T> dPoint)
        {
            if (dPoint == null || dPoint.HeadLink == null) return false;
            var dimension = dPoint.Dimension;
            if (dimension.HeadDimPoint == dPoint || dimension.TailDimPoint == dPoint) return false;
            var head = dPoint.HeadLink;
            var tail = dPoint.TailLink;

            
            // try move right to direct neighbour that has only 1 level
            if (HasBottomLevelNeighbourToTheRight(head))
            {
                var destination = head.Next;

                destination.AssignUpper(head.Upper);
                destination.DimPoint.TailLink = tail;

                head.AssignUpper(null);
                head.DimPoint.TailLink = head.DimPoint.HeadLink;
            }
            // try move left to direct neighbour that has only 1 level
            else if (HasBottomLevelNeighbourToTheLeft(head))
            {
                var destination = head.Prev;

                destination.AssignUpper(head.Upper);
                destination.DimPoint.TailLink = tail;

                head.AssignUpper(null);
                head.DimPoint.TailLink = head.DimPoint.HeadLink;
            }
            // try move the tip right to current level neighbour
            else if (HasTopLevelNeighbourToTheRight(tail))
            {
                var toDelete = tail.Lower.DimPoint;
                var target = tail.Next.Lower.Prev.Prev;
                tail.Lower.AssignUpper(null);
                target.AssignUpper(tail);
                target.DimPoint.TailLink = tail;

                DeleteColumn(toDelete.HeadLink);
            }
            // try move the tip left to current level neighbour
            else if (HasTopLevelNeighbourToTheLeft(tail))
            {
                var toDelete = tail.Lower.DimPoint;
                var target = tail.Prev.Lower.Next.Next;
                tail.Lower.AssignUpper(null);
                target.AssignUpper(tail);
                target.DimPoint.TailLink = tail;

                DeleteColumn(toDelete.HeadLink);
            }
            // delete the column
            else
            {
                DeleteColumn(head);
            }
            
            return true;
        }

        private static void DeleteColumn<T>(DimensionLink<T> head)
        {
            var toSrink = head.Upper;

            // make sure we are not removing edge column
            if (toSrink != null && toSrink.Prev != null && toSrink.Next != null)
            {
                ShrinkColumn(toSrink);
            }
            head.AssignUpper(null);
            head.DimPoint.TailLink = head.DimPoint.HeadLink;
        }


        private static bool HasTopLevelNeighbourToTheLeft<T>(DimensionLink<T> tail)
        {
            return tail.Level > 1 &&
                   tail.Prev != null &&
                   tail.Prev.Lower.Next != tail.Lower &&
                   tail.Prev.Lower.Next.Next != tail.Lower;
        }

        private static bool HasTopLevelNeighbourToTheRight<T>(DimensionLink<T> tail)
        {
            return tail.Level > 1 && 
                   tail.Next != null &&
                   tail.Next.Lower.Prev != tail.Lower &&
                   tail.Next.Lower.Prev.Prev != tail.Lower;
        }

        private static bool HasBottomLevelNeighbourToTheLeft<T>(DimensionLink<T> head)
        {
            var prev = head.Prev;
            return prev != null &&
                   prev.Upper == null &&
                   prev.Prev != null &&
                   prev.Prev.Upper == null;
        }

        private static bool HasBottomLevelNeighbourToTheRight<T>(DimensionLink<T> head)
        {
            var next = head.Next;
            return next != null &&
                   next.Upper == null &&
                   next.Next != null &&
                   next.Next.Upper == null;
        }

        private static void ShrinkColumn<T>(DimensionLink<T> link)
        {
            while (link != null)
            {
                if (link.Prev != null) link.Prev.AssignNext(link.Next);
                else if (link.Next != null) link.Next.AssignPrev(link.Prev);
                link = link.Upper;
            }
        }

        private static void ProcessTwoColumns<T>(DimensionLink<T> a, DimensionLink<T> b, Action<DimensionLink<T>, DimensionLink<T>> func)
        {
            while (a != null && b != null)
            {
                func(a, b);
                a = a.Upper;
                b = b.Upper;
            }
        }

        
        

        SpacePoint<T> ISpaceManager.AddPoint<T>(Space<T> space, T data, params float[] dimensionPositions)
        {
            MustBe.NotNull(space, () => "space");
            MustBe.NotNull(dimensionPositions, () => "dimensionPositions");
            MustBe.Equal(dimensionPositions.Length, space.Dimensions.Length, () => "number of dimensions does not match the number of dimension positions passed when adding point to space");

            var sp = new SpacePoint<T>(space) { Value = data };

            if (dimensionPositions == null) throw new ArgumentException("Expected dimension positions");

            for (var i = 0; i < space.Dimensions.Length; ++i)
            {
                _spaceManager.AddPoint(space.Dimensions[i], sp, dimensionPositions[i]);
            }

            return sp;
        }

        bool ISpaceManager.AddPoint<T>(Dimension<T> dimension, SpacePoint<T> sp, float position)
        {
            if (dimension == null || sp == null)
            {
                return false;
            }

            MustBe.Null(sp.Dimensions[dimension.Index], () => "space point already has assigned the dimension with index="+dimension.Index);

            if (dimension.HeadDimPoint == null)
            {
                var dp = new DimensionPoint<T>(dimension) { Position = position };
                dp.HeadLink = dp.TailLink = new DimensionLink<T>(0, dp);
                dp.AddPoint(sp);
                dimension.HeadDimPoint = dimension.TailDimPoint = dp;
                dimension.Count = 1;
                return true;
            }

            // try to find existing dimension point
            DimensionPoint<T> left;
            DimensionPoint<T> right;
            if (_spaceManager.TryFindDimensionPoint(dimension, position, out left, out right))
            {
                // if found add the space point to it
                return left.AddPoint(sp);
            }
            // new head
            if (left == null)
            {
                AppendNewHead(dimension, sp, position);
                dimension.Count++;
                return true;
            }
            // new tail
            if (right == null)
            {
                AppendNewTail(dimension, sp, position);
                dimension.Count++;
                return true;
            }

            // new in between
            var newPoint = new DimensionPoint<T>(dimension) { Position = position };
            newPoint.AddPoint(sp);
            var newLink = new DimensionLink<T>(0, newPoint);
            newPoint.HeadLink = newLink;
            newPoint.TailLink = newLink;

            left.HeadLink.AssignNext(newLink);
            newLink.AssignNext(right.HeadLink);

            _spaceManager.TryAddLevel(dimension);
            var toExtendUp = newLink.GetSiblingExtensionCandidate();
            while (_spaceManager.TryExtendUp(toExtendUp, out toExtendUp))
            {
                _spaceManager.TryAddLevel(dimension);
            }
            dimension.Count++;
            return true;
        }

        private void AppendNewTail<T>(Dimension<T> dimension, SpacePoint<T> sp, float position)
        {
            var newTail = new DimensionPoint<T>(dimension) {Position = position};
            newTail.AddPoint(sp);
            newTail.HeadLink = new DimensionLink<T>(0, newTail);

            var oldTail = dimension.TailDimPoint;

            dimension.TailDimPoint = newTail;
            oldTail.HeadLink.AssignNext(newTail.HeadLink);

            var upper = oldTail.HeadLink.Upper;
            newTail.HeadLink.AssignUpper(upper);
            oldTail.HeadLink.Upper = null;
            newTail.TailLink = oldTail.TailLink.Level == 0 ? newTail.HeadLink : oldTail.TailLink;
            oldTail.TailLink = oldTail.HeadLink;


            _spaceManager.TryAddLevel(dimension);
            var toExtendUp = dimension.TailDimPoint.HeadLink.PrevUntil((n, i) => i == 1);
            while (_spaceManager.TryExtendUp(toExtendUp, out toExtendUp))
            {
                _spaceManager.TryAddLevel(dimension);
            }
        }

        private void AppendNewHead<T>(Dimension<T> dimension, SpacePoint<T> sp, float position)
        {
            var newHead = new DimensionPoint<T>(dimension) {Position = position};
            newHead.AddPoint(sp);
            newHead.HeadLink = new DimensionLink<T>(0, newHead);

            var oldHead = dimension.HeadDimPoint;

            dimension.HeadDimPoint = newHead;
            newHead.HeadLink.AssignNext(oldHead.HeadLink);

            var upper = oldHead.HeadLink.Upper;
            newHead.HeadLink.AssignUpper(upper);
            oldHead.HeadLink.Upper = null;
            newHead.TailLink = oldHead.TailLink.Level == 0 ? newHead.HeadLink : oldHead.TailLink;
            oldHead.TailLink = oldHead.HeadLink;

            _spaceManager.TryAddLevel(dimension);
            var toExtendUp = dimension.HeadDimPoint.HeadLink.NextUntil((n, i) => i == 1);
            while (_spaceManager.TryExtendUp(toExtendUp, out toExtendUp))
            {
                _spaceManager.TryAddLevel(dimension);
            }
        }

        bool ISpaceManager.TryAddLevel<T>(Dimension<T> dimension)
        {
            if (dimension.HeadDimPoint == null || dimension.HeadDimPoint.TailLink == null) return false;

            var topLeft = dimension.HeadDimPoint.TailLink;

            if (topLeft.CountConnectionsRight() == 0) return false;
            
            var left = new DimensionLink<T>((byte)(topLeft.Level + 1));
            topLeft.AssignUpper(left);

            var topRight = dimension.TailDimPoint.TailLink;
            var right = new DimensionLink<T>((byte)(topRight.Level + 1));
            topRight.AssignUpper(right);

            left.AssignNext(right);

            dimension.HeadDimPoint.TailLink = left;
            dimension.TailDimPoint.TailLink = right;

            return true;
        }

        bool ISpaceManager.TryRemoveLevel<T>(Dimension<T> dimension)
        {
            return TryRemoveLevelInternal(dimension);
        }

        private static bool TryRemoveLevelInternal<T>(Dimension<T> dimension)
        {
            if (dimension.HeadDimPoint == null) return false;

            var status = false;

            while (true)
            {
                var topLeft = dimension.HeadDimPoint.TailLink;

                if (topLeft.Level == 0) break;

                var bottomLeft = topLeft.Lower;

                if (RowHasLinkNotConnectedToUpper(bottomLeft)) break;

                var link = bottomLeft;
                while (link != null)
                {
                    link.AssignUpper(null);
                    var point = link.DimPoint;
                    point.TailLink = link;

                    link = link.Next;
                }

                status = true;
            }
            return status;

        }

        private static bool RowHasLinkNotConnectedToUpper<T>(DimensionLink<T> left)
        {
            var link = left;
            while (link != null)
            {
                if (link.Upper == null) return true;
                link = link.Next;
            }
            return false;
        }

        bool ISpaceManager.TryExtendUp<T>(DimensionLink<T> toExtendUp, out DimensionLink<T> nextUpExtension)
        {
            nextUpExtension = null;
            if (toExtendUp == null || // nothing to extend
                toExtendUp.Upper != null || // alrady extended
                toExtendUp.Next == null ||  // cannot extend up right edge
                toExtendUp.Prev == null || // cannot extend up left edge
                toExtendUp.Next.Upper != null || // no need to extend up if neighbours are extended up
                toExtendUp.Prev.Upper != null)
            {
                return false;
            }

            var leftCorner = toExtendUp.PrevUntil((n, i) => n.Upper != null);
            var rightCorner = toExtendUp.NextUntil((n, i) => n.Upper != null);

            if (leftCorner == null || rightCorner == null)
            {
                return false;
            }

            var left = leftCorner.Upper;
            var right = rightCorner.Upper;

            var upper = new DimensionLink<T>((byte) (toExtendUp.Level + 1));

            toExtendUp.AssignUpper(upper);
            left.AssignNext(upper);
            upper.AssignNext(right);

            toExtendUp.DimPoint.TailLink = upper;

            nextUpExtension = upper.GetSiblingExtensionCandidate();

            return true;
        }
        bool ISpaceManager.TryFindFirstSpacePoint<T>(Space<T> space, out SpacePoint<T> point, params float[] dimensionPositions)
        {
            HashSet<SpacePoint<T>> set;
            _spaceManager.TryFindSpacePoints(space, out set, dimensionPositions);
            point = set.FirstOrDefault();
            return point != null;
        }
        

        bool ISpaceManager.TryFindSpacePoints<T>(Space<T> space, out HashSet<SpacePoint<T>> points, params float[] dimensionPositions)
        {
            MustBe.Equal(space.Dimensions.Length, dimensionPositions.Length, () => "space.Dimensions.Length AND dimensionPositions.Length");

            points = new HashSet<SpacePoint<T>>();
            for (var d = 0; d < space.Dimensions.Length; ++d)
            {
                DimensionPoint<T> left;
                DimensionPoint<T> right;
                if (!_spaceManager.TryFindDimensionPoint(space.Dimensions[d], dimensionPositions[d], out left, out right))
                {
                    return false;
                }
                if (left != null)
                {
                    foreach (var sp in left.SpaPoints)
                    {
                        if (sp.IsLocatedAt(dimensionPositions))
                        {
                            points.Add(sp);
                        }
                    }
                }
            }
            return true;
        }
        bool ISpaceManager.TryFindDimensionPoint<T>(Dimension<T> dimension, float position, out DimensionPoint<T> left, out DimensionPoint<T> right)
        {
            if (dimension.HeadDimPoint == null)
            {
                left = null;
                right = null;
                return false;
            }

            var candidateLeft = dimension.HeadDimPoint.TailLink;
            var candidateRight = dimension.TailDimPoint.TailLink;
            var leftLimit = candidateLeft.DimPoint.Position;
            var rightLimit = candidateRight.DimPoint.Position;
            if (position > (leftLimit - dimension.Epsillon) && position < (rightLimit + dimension.Epsillon))
            {
                while (true)
                {
                    while (candidateLeft.Next != null && candidateLeft.Next != candidateRight)
                    {
                        if ((candidateLeft.Next.DimPoint.Position - dimension.Epsillon) < position)
                        {
                            candidateLeft = candidateLeft.Next;
                        }
                        else
                        {
                            candidateRight = candidateLeft.Next;
                            break;
                        }
                    }

                    if (candidateLeft.Level > 0)
                    {
                        candidateLeft = candidateLeft.Lower;
                        candidateRight = candidateRight.Lower;
                        continue;
                    }


                    if (dimension.Eq(position, candidateLeft.DimPoint.Position))
                    {
                        left = right = candidateLeft.DimPoint;
                        return true;
                    }
                    if (dimension.Eq(position, candidateRight.DimPoint.Position))
                    {
                        left = right = candidateRight.DimPoint;
                        return true;
                    }
                    break;
                }
                left = candidateLeft.DimPoint;
                right = candidateRight.DimPoint;
                return false;
            }
            else if (position < leftLimit)
            {
                left = null;
                right = candidateLeft.DimPoint;
                return false;
            }
            else if (position > rightLimit)
            {
                left = candidateRight.DimPoint;
                right = null;
                return false;
            }


            left = null;
            right = null;
            return false;
        }

        bool ISpaceManager.Reposition<T>(SpacePoint<T> sp, float x, float y, float z)
        {
            MustBe.Equal(sp.Dimensions.Length, 3, () => "space.Dimensions.Length AND x,y,z");
            byte r = 0;
            if (_spaceManager.Reposition(sp.Dimensions[0].Dimension, sp, x)) ++r;
            if (_spaceManager.Reposition(sp.Dimensions[1].Dimension, sp, y)) ++r;
            if (_spaceManager.Reposition(sp.Dimensions[2].Dimension, sp, z)) ++r;
            return r > 0;
        }
        bool ISpaceManager.Reposition<T>(SpacePoint<T> sp, params float[] dimensionPositions)
        {
            MustBe.Equal(sp.Dimensions.Length, dimensionPositions.Length, () => "space.Dimensions.Length AND dimensionPositions.Length");
            byte r = 0;
            for (var d = 0; d < sp.Dimensions.Length; ++d)
            {
                if (_spaceManager.Reposition(sp.Dimensions[d].Dimension, sp, dimensionPositions[d])) ++r;
            }
            return r > 0;
        }
        bool ISpaceManager.Reposition<T>(Dimension<T> dimension, SpacePoint<T> sp, float position)
        {
            var point = sp.Dimensions[dimension.Index];

            var next = position;
            var prev = point.Position;

            // no move
            if (dimension.Eq(prev, next)) return false;

            if (next < prev)
            {
                if (point.NumberPoints > 1)
                {
                    if (RemovePointFromDimensions(sp, dimension.Index))
                    {
                        sp.Dimensions[dimension.Index] = null;
                        _spaceManager.AddPoint(dimension, sp, next);
                    }
                }
                else
                {
                    var link = point.HeadLink;
                    if (link.Prev == null || link.Prev.DimPoint.Position < next)
                    {
                        point.Position = next;
                    }
                    else
                    {
                        if (RemovePointFromDimensions(sp, dimension.Index))
                        {
                            sp.Dimensions[dimension.Index] = null;
                            _spaceManager.AddPoint(dimension, sp, next);
                        }
                    }
                }

                
            }
            else // THIS IS: if (prev < next)
            {

                if (point.NumberPoints > 1)
                {
                    if (RemovePointFromDimensions(sp, dimension.Index))
                    {
                        sp.Dimensions[dimension.Index] = null;
                        _spaceManager.AddPoint(dimension, sp, next);
                    }
                }
                else
                {
                    var link = point.HeadLink;
                    if (link.Next == null || link.Next.DimPoint.Position > next)
                    {
                        point.Position = next;
                    }
                    else
                    {
                        if (RemovePointFromDimensions(sp, dimension.Index))
                        {
                            sp.Dimensions[dimension.Index] = null;
                            _spaceManager.AddPoint(dimension, sp, next);
                        }
                    }
                }
            }

            return true;
        }

        void ISpaceManager.ClearSpace<T>(Space<T> space)
        {
            for (var i = 0; i < space.Dimensions.Length; ++i)
            {
                ClearDimension(space.Dimensions[i]);
            }
        }

        private void ClearDimension<T>(Dimension<T> dimension)
        {
            dimension.HeadDimPoint = dimension.TailDimPoint = null;
            dimension.Count = 0;
        }

        void ISpaceManager.PopulateSpace<T>(Space<T> space, List<SpacePointSource<T>> points)
        {
            _spaceManager.ClearSpace(space);
            for (var i = 0; i < space.Dimensions.Length; ++i)
            {
                PopulateDimension(space.Dimensions[i], points);
            }
//            for (var i = 0; i < points.Count; ++i)
//            {
//                points[i].InitialPositions = null;
//            }

        }
        private bool PopulateDimension<T>(Dimension<T> dimension, List<SpacePointSource<T>> points)
        {
            if(points.Count == 0) return false;

            points.Sort((a, b) => a.InitialPositions[dimension.Index].CompareTo(b.InitialPositions[dimension.Index]));

            DimensionPoint<T> head = null;
            DimensionPoint<T> curr = null;
            DimensionPoint<T> prev = null;

            for (var i = 0; i < points.Count; ++i)
            {
                var sp = points[i];
                if (sp == null) continue;
                var position = sp.InitialPositions[dimension.Index];
                if (prev != null)
                {
                    if (dimension.Eq(prev.Position, position))
                    {
                        prev.AddPoint(sp);
                        continue;
                    }
                }
                curr = new DimensionPoint<T>(dimension) {Position = position};
                curr.HeadLink = new DimensionLink<T>(0, curr);
                curr.TailLink = curr.HeadLink;
                curr.AddPoint(sp);

                if (prev != null)
                {
                    prev.HeadLink.AssignNext(curr.HeadLink);
                }
                else
                {
                    head = curr;
                }
                prev = curr;
            }

            dimension.HeadDimPoint = head;
            dimension.TailDimPoint = curr;
            dimension.Count += points.Count;

// ReSharper disable PossibleNullReferenceException
            DimensionLink<T> link = dimension.HeadDimPoint.HeadLink;
// ReSharper restore PossibleNullReferenceException

            // if this is the top level
            if (link.Next == null || link.Next.Next == null) return true;

            DimensionLink<T> prevUpper = null;
            DimensionLink<T> firstUpper = null;
            while (link != null)
            {
                var upper = new DimensionLink<T>((byte) (link.Level + 1));
                if (firstUpper == null)
                {
                    firstUpper = upper;
                }
                link.AssignUpper(upper);
                link.DimPoint.TailLink = upper;
                if (prevUpper != null)
                {
                    prevUpper.AssignNext(upper);
                }
                // if this is the end
                if (link.Next == null)
                {
                    link = firstUpper;
                    firstUpper = null;
                    prevUpper = null;
                    // if this is the top level
                    if (link.Next == null || link.Next.Next == null) return true;
                    continue;
                }
                link = link.Next.Next;
                // the next is the end and it is two links away
                if (link.Next != null && link.Next.Next == null)
                {
                    link = link.Next;
                }
                prevUpper = upper;
            }
            return true;
        }

    }
}
