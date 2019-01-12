namespace Bones
{
    using System;
    using System.Collections.Generic;
    using System.Threading;


    public interface IAdvancedScope : IScope
    {
        ContractRegistry Contracts { get; }
        
        ICache<string, Instance> InstanceCache { get; }
        
        Stack<Instance> Tracked { get; }

        string Name { get; }

        object Resolve(ServiceKey serviceKey);

        object Resolve(Contract contract);

        Scope ParentScope { get; }
    }

    public class Scope : IAdvancedScope
    {
        public Scope(ContractRegistry contractRegistry, Scope parentScope = null, string name = "scope")
        {
            Name = name;
            Contracts = contractRegistry ?? throw new ArgumentNullException(nameof(contractRegistry));
            ParentScope = parentScope;
            InstanceCache = new SimpleCache<string, Instance>(5);
            Tracked = new Stack<Instance>(10);
        }

        public ICache<string, Instance> InstanceCache { get; }

        public Stack<Instance> Tracked { get; }

        public ContractRegistry Contracts { get; }
        public Scope ParentScope { get; }
        public string Name { get; }

        public object Resolve(ServiceKey serviceKey)
        {
            Code.Require(()=> serviceKey != null, nameof(serviceKey));

            var contract = Contracts.GetContract(serviceKey);
            return contract.LifeSpan.Resolve(this, contract);
        }
        
        public object Resolve(Contract contract)
        {
            Code.Require(()=> contract != null, nameof(contract));
            return contract.LifeSpan.Resolve(this, contract);
        }

        public TService Resolve<TService>(string serviceName = "default")
        {
            return (TService) Resolve(new ServiceKey(typeof(TService), serviceName));
        }

        public object Resolve(Type service, string name = "default")
        {
            Code.Require(() => service != null, nameof(service));
            
            return Resolve(new ServiceKey(service, name));
        }


        public void Dispose()
        {
            while (Tracked.TryPop(out var instance))
            {
                instance.Contract.DisposeInstance(instance.Value);
            }
        }

        public IScope CreateScope(string name = "scope")
        {
            return new Scope(Contracts, this, name);
        }
    }

    public static class Code 
    {
        public static void Require<T>(Func<bool> predicate, Func<T> ex) where T: Exception
        {
            if (predicate()) return;
            throw ex();
        }

        public static void Require(Func<bool> predicate, string name)
        {
            if (predicate()) return;
            throw new ArgumentException(name);
        }

        public static void Ensure<T>(Func<bool> predicate, Func<T> ex) where T: Exception
        {
            if (predicate()) return;
            throw ex();
        }

    }


    public class Instance
    {
        public Contract Contract { get; set; }
        public object Value { get; set; }
    }


    public interface ICache<TKey, TValue> where TValue : class
    {
        int Count { get; }
        TValue Get(TKey key);
        void Add(TKey key, TValue value);
        void Delete(TKey key);
    }

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
        
        public int Count { get; }
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


    /// <summary>
    /// a unique key for a service
    /// </summary>
    public class ServiceKey
    {
        private readonly string _internalName;
        private readonly int _hash;

        public ServiceKey(Type service, string serviceName = "default")
        {
            Code.Require(()=> service != null, nameof(service));
            
            ServiceName = serviceName;
            Service = service;
            _internalName = $"{Service.FullName} ^_^ {ServiceName}";
            _hash = _internalName.GetHashCode();
        }

        public string ServiceName { get; protected set; }
        public Type Service { get; protected set; }

        public override string ToString()
        {
            return $"Key: {_internalName}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return _hash == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hash;
        }
    }
}