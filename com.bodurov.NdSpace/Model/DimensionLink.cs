using System;

namespace com.bodurov.NdSpace.Model
{
    public class DimensionLink<T>
    {
        private DimensionPoint<T> _point;
        public DimensionLink(byte level, DimensionPoint<T> point = null)
        {
            Level = level;
            if (level > 0 && point != null) throw new InvalidOperationException("Cannot assign point to level higher than 0");
            _point = point;
        }

        public DimensionLink<T> Prev { get; internal set; }
        public DimensionLink<T> Next { get; internal set; }
        public DimensionLink<T> Upper { get; internal set; }
        public DimensionLink<T> Lower { get; internal set; }
        public DimensionPoint<T> Point
        {
            get
            {
                if (Level > 0)
                {
                    if (Lower == null) throw new InvalidOperationException("Missing lower link");
                    return Lower.Point;
                }
                return  _point;
            }
            internal set { _point = value; }
        }
        public float Position { get { return _point == null ? Single.NaN : _point.Position; } }
        public byte Level { get; private set; }


        public DimensionLink<T> AssignUpper(DimensionLink<T> upper)
        {
            if(upper != null) upper.Lower = this;
            Upper = upper;
            return this;
        }
        public DimensionLink<T> AssignNext(DimensionLink<T> next)
        {
            if (next != null) next.Prev = this;
            Next = next;
            return this;
        }
        public DimensionLink<T> AssignPrev(DimensionLink<T> prev)
        {
            if (prev != null) prev.Next = this;
            Prev = prev;
            return this;
        }

        public DimensionLink<T> NextUntil(Func<DimensionLink<T>,int,bool> condition)
        {
            var i = 0;
            var curr = Next;
            while (curr != null)
            {
                if (condition(curr, i++)) return curr;
                curr = curr.Next;
            }
            return null;
        }
        public DimensionLink<T> PrevUntil(Func<DimensionLink<T>, int, bool> condition)
        {
            var i = 0;
            var curr = Prev;
            while (curr != null)
            {
                if (condition(curr, i++)) return curr;
                curr = curr.Prev;
            }
            return null;
        }
        public DimensionLink<T> UpperUntil(Func<DimensionLink<T>, int, bool> condition)
        {
            var i = 0;
            var curr = Upper;
            while (curr != null)
            {
                if (condition(curr, i++)) return curr;
                curr = curr.Upper;
            }
            return null;
        }
        public DimensionLink<T> LowerUntil(Func<DimensionLink<T>, int, bool> condition)
        {
            var i = 0;
            var curr = Lower;
            while (curr != null)
            {
                if (condition(curr, i++)) return curr;
                curr = curr.Lower;
            }
            return null;
        }

        public int CountConnectionsRight()
        {
            var i = 0;
            var next = Next;
            while (next != null)
            {
                if (next.Upper == null && next.Next != null && next.Prev != null) ++i;
                else break;

                next = next.Next;
            }
            return i;
        }
        public int CountConnectionsLeft()
        {
            var i = 0;
            var prev = Prev;
            while (prev != null)
            {
                if (prev.Upper == null && prev.Next != null && prev.Prev != null) ++i;
                else break;

                prev = prev.Next;
            }
            return i;
        }

        public DimensionLink<T> GetSiblingExtensionCandidate()
        {
            var leftNode = PrevUntil((n, i) => n.Upper != null);
            if (leftNode == null) return null;
            var candidate = leftNode.NextUntil((n, i) => i == 1 && n.Upper == null && n.Next != null && n.Next.Upper == null);
            if (candidate == null)
            {
                var rightNode = NextUntil((n, i) => n.Upper != null);
                if (rightNode == null) return null;
                candidate = rightNode.PrevUntil((n, i) => i == 1 && n.Upper == null && n.Prev != null && n.Prev.Upper == null);
            }
            return candidate;
        }

        public override string ToString()
        {
            return Point != null ? "DimensionLink("+Position+")" : base.ToString();
        }
        
    }
}