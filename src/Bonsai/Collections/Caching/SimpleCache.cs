namespace Bonsai.Collections.Caching
{
    using System.Collections.Generic;

    public class SimpleCache<TKey, TValue> : ICache<TKey, TValue> where TValue : class
    {
        private readonly Dictionary<TKey, TValue> _innerCache;

        public SimpleCache(int capacity)
        {
            _innerCache = new Dictionary<TKey, TValue>(capacity);
        }

        public SimpleCache()
        {
            _innerCache = new Dictionary<TKey, TValue>();
        }
        
        public TValue Get(TKey key)
        {
            return _innerCache.TryGetValue(key, out var v) 
                ? v 
                : null;
        }

        public void Add(TKey key, TValue value)
        {
            _innerCache.Add(key, value);
        }

        public void Delete(TKey key)
        {
            _innerCache.Remove(key);
        }
    }
}