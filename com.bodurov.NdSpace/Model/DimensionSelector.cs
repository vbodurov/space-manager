using System;
using com.bodurov.NdSpace.Interface;

namespace com.bodurov.NdSpace.Model
{
    public class DimensionSelector : IDimensionSelector
    {
        public static readonly IDimensionSelector Default = new DimensionSelector(0, i => true);

        private readonly int _mainIndex;
        private readonly Func<int, bool> _hasDimension;

        public DimensionSelector(int mainIndex, Func<int,bool> hasDimension)
        {
            _mainIndex = mainIndex;
            _hasDimension = hasDimension;
        }

        int IDimensionSelector.MainDimensionIndex { get { return _mainIndex; } }
        bool IDimensionSelector.IncludesDimension(int index) { return _hasDimension(index); }
    }
}