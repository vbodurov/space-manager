using System.Threading;

namespace com.bodurov.NdSpace.Model
{
    public abstract class BaseSpaceObject
    {
        private static long _lastId = 0;
        protected BaseSpaceObject()
        {
            ID = Interlocked.Increment(ref _lastId);
        }
        public long ID { get; private set; }
        //public object Tag { get; set; }
    }
}