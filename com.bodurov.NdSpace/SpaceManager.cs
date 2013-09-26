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
        private readonly ISpaceConfig _config;


        internal SpaceManager(ISpaceConfig config)
        {
            _spaceManager = this;
            _config = config;
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
            var centerLink = centerDimPoint.Head;
            if (centerDimPoint.NumberPoints > 1)
            {
                foreach (var sp in centerDimPoint.Points.Where(p => p.ID != center.ID))
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
                foreach(var nextSpaPoint in next.Point.Points)
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
                foreach(var prevSpaPoint in prev.Point.Points)
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
            var centerLink = centerDimPoint.Head;
            if (centerDimPoint.NumberPoints > 1)
            {
                foreach (var sp in centerDimPoint.Points.Where(p => p.ID != center.ID))
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
                foreach (var nextSpaPoint in next.Point.Points)
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
                foreach (var prevSpaPoint in prev.Point.Points)
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
            var centerLink = centerDimPoint.Head;
            if (centerDimPoint.NumberPoints > 1)
            {
                foreach (var sp in centerDimPoint.Points.Where(p => p.ID != center.ID))
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
                foreach (var nextSpaPoint in next.Point.Points)
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
                foreach (var prevSpaPoint in prev.Point.Points)
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
            var space = new Space<T>(config ?? _config);

            for (byte i = 0; i < space.Dimensions.Length; ++i)
            {
                space.Dimensions[i] = new Dimension<T>(i, space);
            }

            return space;
        }

        bool ISpaceManager.RemovePoint<T>(SpacePoint<T> sp)
        {
            var isRemoved = false;
            for (var i = 0; i < sp.Dimensions.Length; ++i)
            {
                var dPoint = sp.Dimensions[i];
                if (dPoint == null)
                {
                    continue;
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

                sp.Dimensions[i] = null;
            }
            return isRemoved;
        }

        private static bool RemoveDimensionPoint<T>(DimensionPoint<T> dPoint)
        {
            var dimension = dPoint.Dimension;
            if (dimension == null || dimension.Head == null)
            {
                throw new InvalidOperationException("Invalid tree: point was requested to be removed but the tree is already empty");
            }
            if (dimension.Count == 1)
            {
                if (dimension.Head == dPoint)
                {
                    dimension.Count = 0;
                    dimension.Head = dimension.Tail = null;
                    return true;
                }
                throw new InvalidOperationException("Invalid tree: point requested to be removed does not seem to belong to the tree");
            }
            if (dimension.Head == dPoint)
            {
                RemoveHead(dPoint.Dimension);
            }
            else if (dimension.Tail == dPoint)
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
            var oldHead = dimension.Head;
            var oldHeadLevel0 = oldHead.Head;
            var newHead = oldHeadLevel0.Next.Point;
            ProcessTwoColumns(oldHead.Head, newHead.Head, (a, b) => a.AssignNext(b.Next));


            newHead.Head = oldHead.Head;
            newHead.Tail = oldHead.Tail;
            dimension.Head = newHead;

            if (dimension.Head.Head.Next != null) RemoveOrMoveColumn(dimension.Head.Head.Next.Point);
            TryRemoveLevelInternal(dimension);
        }

        

        private static void RemoveTail<T>(Dimension<T> dimension)
        {
            var oldTail = dimension.Tail;
            var oldTailLevel0 = oldTail.Head;
            var newTail = oldTailLevel0.Prev.Point;

            ProcessTwoColumns(newTail.Head, oldTail.Head, (a, b) => b.AssignPrev(a.Prev));

            newTail.Head = oldTail.Head;
            newTail.Tail = oldTail.Tail;
            dimension.Tail = newTail;

            if (dimension.Tail.Head.Prev != null) RemoveOrMoveColumn(dimension.Tail.Head.Prev.Point);
            TryRemoveLevelInternal(dimension);
        }
        private static void RemoveMiddlePoint<T>(DimensionPoint<T> dPoint)
        {
            var link = dPoint.Head;

            RemoveOrMoveColumn(dPoint);

            var prev = link.Prev;
            var next = link.Next;

            prev.AssignNext(next);
            TryRemoveLevelInternal(dPoint.Dimension);
        }

        
        private static bool RemoveOrMoveColumn<T>(DimensionPoint<T> dPoint)
        {
            if (dPoint == null || dPoint.Head == null) return false;
            var dimension = dPoint.Dimension;
            if (dimension.Head == dPoint || dimension.Tail == dPoint) return false;
            var head = dPoint.Head;
            var tail = dPoint.Tail;

            
            // try move right to direct neighbour that has only 1 level
            if (HasBottomLevelNeighbourToTheRight(head))
            {
                var destination = head.Next;

                destination.AssignUpper(head.Upper);
                destination.Point.Tail = tail;

                head.AssignUpper(null);
                head.Point.Tail = head.Point.Head;
            }
            // try move left to direct neighbour that has only 1 level
            else if (HasBottomLevelNeighbourToTheLeft(head))
            {
                var destination = head.Prev;

                destination.AssignUpper(head.Upper);
                destination.Point.Tail = tail;

                head.AssignUpper(null);
                head.Point.Tail = head.Point.Head;
            }
            // try move the tip right to current level neighbour
            else if (HasTopLevelNeighbourToTheRight(tail))
            {
                var toDelete = tail.Lower.Point;
                var target = tail.Next.Lower.Prev.Prev;
                tail.Lower.AssignUpper(null);
                target.AssignUpper(tail);
                target.Point.Tail = tail;

                DeleteColumn(toDelete.Head);
            }
            // try move the tip left to current level neighbour
            else if (HasTopLevelNeighbourToTheLeft(tail))
            {
                var toDelete = tail.Lower.Point;
                var target = tail.Prev.Lower.Next.Next;
                tail.Lower.AssignUpper(null);
                target.AssignUpper(tail);
                target.Point.Tail = tail;

                DeleteColumn(toDelete.Head);
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
            head.Point.Tail = head.Point.Head;
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

            if (dimension.Head == null)
            {
                var dp = new DimensionPoint<T>(dimension) { Position = position };
                dp.Head = dp.Tail = new DimensionLink<T>(0, dp);
                dp.AddPoint(sp);
                dimension.Head = dimension.Tail = dp;
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
            newPoint.Head = newLink;
            newPoint.Tail = newLink;

            left.Head.AssignNext(newLink);
            newLink.AssignNext(right.Head);

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
            newTail.Head = new DimensionLink<T>(0, newTail);

            var oldTail = dimension.Tail;

            dimension.Tail = newTail;
            oldTail.Head.AssignNext(newTail.Head);

            var upper = oldTail.Head.Upper;
            newTail.Head.AssignUpper(upper);
            oldTail.Head.Upper = null;
            newTail.Tail = oldTail.Tail.Level == 0 ? newTail.Head : oldTail.Tail;
            oldTail.Tail = oldTail.Head;


            _spaceManager.TryAddLevel(dimension);
            var toExtendUp = dimension.Tail.Head.PrevUntil((n, i) => i == 1);
            while (_spaceManager.TryExtendUp(toExtendUp, out toExtendUp))
            {
                _spaceManager.TryAddLevel(dimension);
            }
        }

        private void AppendNewHead<T>(Dimension<T> dimension, SpacePoint<T> sp, float position)
        {
            var newHead = new DimensionPoint<T>(dimension) {Position = position};
            newHead.AddPoint(sp);
            newHead.Head = new DimensionLink<T>(0, newHead);

            var oldHead = dimension.Head;

            dimension.Head = newHead;
            newHead.Head.AssignNext(oldHead.Head);

            var upper = oldHead.Head.Upper;
            newHead.Head.AssignUpper(upper);
            oldHead.Head.Upper = null;
            newHead.Tail = oldHead.Tail.Level == 0 ? newHead.Head : oldHead.Tail;
            oldHead.Tail = oldHead.Head;

            _spaceManager.TryAddLevel(dimension);
            var toExtendUp = dimension.Head.Head.NextUntil((n, i) => i == 1);
            while (_spaceManager.TryExtendUp(toExtendUp, out toExtendUp))
            {
                _spaceManager.TryAddLevel(dimension);
            }
        }

        bool ISpaceManager.TryAddLevel<T>(Dimension<T> dimension)
        {
            if (dimension.Head == null || dimension.Head.Tail == null) return false;

            var topLeft = dimension.Head.Tail;

            if (topLeft.CountConnectionsRight() == 0) return false;
            
            var left = new DimensionLink<T>((byte)(topLeft.Level + 1));
            topLeft.AssignUpper(left);

            var topRight = dimension.Tail.Tail;
            var right = new DimensionLink<T>((byte)(topRight.Level + 1));
            topRight.AssignUpper(right);

            left.AssignNext(right);

            dimension.Head.Tail = left;
            dimension.Tail.Tail = right;

            return true;
        }

        bool ISpaceManager.TryRemoveLevel<T>(Dimension<T> dimension)
        {
            return TryRemoveLevelInternal(dimension);
        }

        private static bool TryRemoveLevelInternal<T>(Dimension<T> dimension)
        {
            if (dimension.Head == null) return false;

            var status = false;

            while (true)
            {
                var topLeft = dimension.Head.Tail;

                if (topLeft.Level == 0) break;

                var bottomLeft = topLeft.Lower;

                if (RowHasLinkNotConnectedToUpper(bottomLeft)) break;

                var link = bottomLeft;
                while (link != null)
                {
                    link.AssignUpper(null);
                    var point = link.Point;
                    point.Tail = link;

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

            toExtendUp.Point.Tail = upper;

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
            if (space.Dimensions.Length != dimensionPositions.Length)
                throw new ArgumentException("Space dimension number of "+space.Dimensions.Length +" does not much passed coordinates number of " + dimensionPositions.Length);
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
                    foreach (var sp in left.Points)
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
            if (dimension.Head == null)
            {
                left = null;
                right = null;
                return false;
            }

            var candidateLeft = dimension.Head.Tail;
            var candidateRight = dimension.Tail.Tail;
            var leftLimit = candidateLeft.Point.Position;
            var rightLimit = candidateRight.Point.Position;
            if (position > (leftLimit - dimension.Epsillon) && position < (rightLimit + dimension.Epsillon))
            {
                while (true)
                {
                    while (candidateLeft.Next != null && candidateLeft.Next != candidateRight)
                    {
                        if ((candidateLeft.Next.Point.Position - dimension.Epsillon) < position)
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


                    if (dimension.Eq(position, candidateLeft.Point.Position))
                    {
                        left = right = candidateLeft.Point;
                        return true;
                    }
                    if (dimension.Eq(position, candidateRight.Point.Position))
                    {
                        left = right = candidateRight.Point;
                        return true;
                    }
                    break;
                }
                left = candidateLeft.Point;
                right = candidateRight.Point;
                return false;
            }
            else if (position < leftLimit)
            {
                left = null;
                right = candidateLeft.Point;
                return false;
            }
            else if (position > rightLimit)
            {
                left = candidateRight.Point;
                right = null;
                return false;
            }


            left = null;
            right = null;
            return false;
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
                    if (point.RemovePoint(sp))
                    {
                        sp.Dimensions[dimension.Index] = null;
                        _spaceManager.AddPoint(dimension, sp, next);
                    }
                }
                else
                {
                    var link = point.Head;
                    if (link.Prev == null || link.Prev.Point.Position < next)
                    {
                        point.Position = next;
                    }
                    else
                    {
                        if (_spaceManager.RemovePoint(sp))
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
                    if (point.RemovePoint(sp))
                    {
                        sp.Dimensions[dimension.Index] = null;
                        _spaceManager.AddPoint(dimension, sp, next);
                    }
                }
                else
                {
                    var link = point.Head;
                    if (link.Next == null || link.Next.Point.Position > next)
                    {
                        point.Position = next;
                    }
                    else
                    {
                        if (_spaceManager.RemovePoint(sp))
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
            dimension.Head = dimension.Tail = null;
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
                curr.Head = new DimensionLink<T>(0, curr);
                curr.Tail = curr.Head;
                curr.AddPoint(sp);

                if (prev != null)
                {
                    prev.Head.AssignNext(curr.Head);
                }
                else
                {
                    head = curr;
                }
                prev = curr;
            }

            dimension.Head = head;
            dimension.Tail = curr;
            dimension.Count += points.Count;

// ReSharper disable PossibleNullReferenceException
            DimensionLink<T> link = dimension.Head.Head;
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
                link.Point.Tail = upper;
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
