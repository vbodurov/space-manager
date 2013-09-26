using System.Collections.Generic;
using System.Linq;

namespace com.bodurov.NdSpace.Model
{
    public class DimensionPoint<T> : BaseSpaceObject
    {
        private readonly HashSet<SpacePoint<T>> _points;

        public DimensionPoint(Dimension<T> dimension)
        {
            Dimension = dimension;

            _points = new HashSet<SpacePoint<T>>();
        }

        public float Position { get; internal set; }
        private DimensionLink<T> _head;
        public DimensionLink<T>  Head
        {
            get { return _head; }
            internal set
            {
                value.Point = this;
                _head = value;
            }
        }
        public DimensionLink<T> Tail { get; internal set; }
        
        public Dimension<T> Dimension { get; private set; }

        public bool AddPoint(SpacePoint<T> sp)
        {
            sp.Dimensions[Dimension.Index] = this;
            return _points.Add(sp);
        }
        public bool RemovePoint(SpacePoint<T> sp)
        {
            return _points.Remove(sp);
        }
        public IEnumerable<SpacePoint<T>> Points
        {
            get
            {
                var e = _points.GetEnumerator();
                while (e.MoveNext())
                {
                    yield return e.Current;
                }
            }
        }
        public SpacePoint<T> FirstPoint
        {
            get { return _points.FirstOrDefault(); }
        }

        public int NumberPoints
        {
            get { return _points.Count; }
        }

        public override string ToString()
        {
            return "DimensionPoint(" + Position + ")";
        }
    }
}