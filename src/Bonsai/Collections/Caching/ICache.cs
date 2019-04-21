namespace Bonsai.Collections.Caching
{
    public interface ICache<in TKey, TValue> where TValue : class
    {
        TValue Get(TKey key);
        void Add(TKey key, TValue value);
        void Delete(TKey key);
    }
}