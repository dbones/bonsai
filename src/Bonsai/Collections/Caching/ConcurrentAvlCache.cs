namespace Bonsai.Collections.Caching
{
    using System.Threading;
    using ImTools;

    public class ConcurrentAvlCache<TKey,TValue> : ICache<TKey, TValue> where TValue : class
    {
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private ImHashMap<TKey, TValue> _innerCache = ImHashMap<TKey, TValue>.Empty;

        public TValue Get(TKey key)
        {
            _cacheLock.EnterReadLock();
            try
            {
                return _innerCache.TryFind(key, out var v) 
                    ? v 
                    : null;
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        public bool TryGet(TKey key, out TValue value)
        {
            _cacheLock.EnterReadLock();
            try
            {
                return _innerCache.TryFind(key, out value);
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
                _innerCache = _innerCache.AddOrUpdate(key, value);
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
                _innerCache = _innerCache.Remove(key);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        ~ConcurrentAvlCache()
        {
            _cacheLock?.Dispose();
        }
    }
}