namespace Ext
{
    public interface IReadonlyIndexable<T>
    {
        T Index { get; }
    }

    public interface IIndexable<T> : IReadonlyIndexable<T>
    {
        new T Index { set; get; }
    }
}
