namespace Bonsai.Collections.LinkedLists
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class ConcurrentLinkedList<T> : LinkedList<T>
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public override void Add(T item)
        {
            try
            {
                _lock.EnterWriteLock();
                base.Add(item);

            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public override IEnumerable<T> GetAll()
        {
            try
            {
                _lock.TryEnterReadLock(200);
                return base.GetAll().ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }

        }
    }
}