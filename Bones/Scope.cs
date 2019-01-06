namespace Bones
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using System.Security;
    using System.Threading;
    using Exceptions;


    public interface IAdvancedScope : IScope
    {
        ContractRegistry Contracts { get; }
        
        Cache<string, Instance> InstanceCache { get; }
        
        Stack<Instance> Tracked { get; }

        string Name { get; }

        object Resolve(ServiceKey serviceKey);

        Scope ParentScope { get; }
    }

    public class Scope : IAdvancedScope
    {
        public Scope(ContractRegistry contractRegistry, Scope parentScope = null, string name = "scope")
        {
            Name = name;
            Contracts = contractRegistry ?? throw new ArgumentNullException(nameof(contractRegistry));
            ParentScope = parentScope;
            InstanceCache = new Cache<string, Instance>();
            Tracked = new Stack<Instance>(10);
        }

        public Cache<string, Instance> InstanceCache { get; }

        public Stack<Instance> Tracked { get; }

        public ContractRegistry Contracts { get; }
        public Scope ParentScope { get; }
        public string Name { get; }

        public object Resolve(ServiceKey serviceKey)
        {
            var contract = Contracts.GetContract(serviceKey);
            return contract.LifeSpan.Resolve(this, contract);
        }

        public TService Resolve<TService>(string serviceName = "default")
        {
            return (TService) Resolve(new ServiceKey(typeof(TService), serviceName));
        }

        public object Resolve(Type service, string name)
        {
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

    public class Instance
    {
        public Contract Contract { get; set; }
        public object Value { get; set; }
    }
    

    public class Cache<TKey,TValue> where TValue : class
    {
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _innerCache;

        public Cache(int capacity)
        {
            _innerCache = new Dictionary<TKey,TValue>(capacity);
        }

        public Cache()
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

        public bool AddWithTimeout(TKey key, TValue value, int timeout)
        {
            if (_cacheLock.TryEnterWriteLock(timeout))
            {
                try
                {
                    _innerCache.Add(key, value);
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        public TValue GetOrAdd(TKey key, Func<TValue> value)
        {
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                TValue result;
                if (_innerCache.TryGetValue(key, out result))
                {
                    return result;
                }
                else
                {
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        var v = value();
                        _innerCache.Add(key, v);
                        return v;
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        public AddOrUpdateStatus AddOrUpdate(TKey key, TValue value)
        {
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                TValue result;
                if (_innerCache.TryGetValue(key, out result))
                {
                    if (result.Equals(value))
                    {
                        return AddOrUpdateStatus.Unchanged;
                    }
                    else
                    {
                        _cacheLock.EnterWriteLock();
                        try
                        {
                            _innerCache[key] = value;
                        }
                        finally
                        {
                            _cacheLock.ExitWriteLock();
                        }

                        return AddOrUpdateStatus.Updated;
                    }
                }
                else
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

                    return AddOrUpdateStatus.Added;
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
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

        public enum AddOrUpdateStatus
        {
            Added,
            Updated,
            Unchanged
        };

        ~Cache()
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