namespace Bonsai.Collections.Caching
{
    using System.Collections.Generic;
    using System.Threading;

    public class ConcurrentCache<TKey,TValue> : ICache<TKey, TValue> where TValue : class
    {
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _innerCache;

        public ConcurrentCache(int capacity)
        {
            _innerCache = new Dictionary<TKey,TValue>(capacity);
        }

        public ConcurrentCache()
        {
            _innerCache = new Dictionary<TKey,TValue>();
        }
        
        public int Count => _innerCache.Count;

        public TValue Get(TKey key)
        {
            _cacheLock.EnterReadLock();
            try
            {
                return _innerCache.TryGetValue(key, out var v) 
                    ? v 
                    : null;
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        public void Add(TKey key, TValue value)
        {
            _cacheLock.EnterWriteLock();
            try
            {
                _innerCache.Add(key, value);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        public void Delete(TKey key)
        {
            _cacheLock.EnterWriteLock();
            try
            {
                _innerCache.Remove(key);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        ~ConcurrentCache()
        {
            _cacheLock?.Dispose();
        }
    }
}