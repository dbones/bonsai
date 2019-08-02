namespace Bonsai.Collections.Caching
{
    public interface ICache<in TKey, TValue> where TValue : class
    {
        //TValue Get(TKey key);

        bool TryGet(TKey key, out TValue value);

        void Add(TKey key, TValue value);
        void Delete(TKey key);
    }
}