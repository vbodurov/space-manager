using System;
using com.bodurov.NdSpace.Interface;
using com.bodurov.NdSpace.Model;

namespace YouVisio.Unity3D.AI.Tests.SampleExtensions
{
    public static class Ext
    {
        public static SpacePoint<string> FindNearest(this ISpaceManager sm, SpacePoint<string> center, float within)
        {
            var pan = new PointNfo<string>(null, Single.MaxValue);
            return sm.AggregateWithin(center, pan, within, (aggr, curr, cen, dis) =>
            {
                if (dis < aggr.Distance)
                {
                    aggr.Point = curr;
                    aggr.Distance = dis;
                }
                return aggr;
            }).Point;
        }
    }
}