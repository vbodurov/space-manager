namespace com.bodurov.NdSpace.Interface
{
    public interface IDimensionSelector
    {
        int MainDimensionIndex { get; }
        bool IncludesDimension(int index);
    }
}