namespace Bonsai.Collections.Caching
{
    using ImTools;

    public class AvlCache<TKey, TValue> : ICache<TKey, TValue> where TValue : class
    {
        private ImHashMap<TKey, TValue> _innerCache = ImHashMap<TKey, TValue>.Empty;

        public TValue Get(TKey key)
        {
            return _innerCache.TryFind(key, out var v) 
                ? v 
                : null;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            return _innerCache.TryFind(key, out value);
        }


        public void Add(TKey key, TValue value)
        {
            _innerCache = _innerCache.AddOrUpdate(key,value);
        }

        public void Delete(TKey key)
        {
            _innerCache = _innerCache.Remove(key);
        }
    }
}