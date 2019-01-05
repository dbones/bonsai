namespace Bones
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using Exceptions;


    public interface IInternalScopeApi
    {
        ContractRegistry Contracts { get; }
        
        string Name { get; }

        object Resolve(ServiceKey serviceKey);
    }
    
    public class Scope : IScope, IInternalScopeApi
    {
        //private List<InstanceEntry> _nonReuseableEntries;
        //private IDictionary<string, InstanceEntry> _instanceEntries;
        private Scope _parentScope;

        public Scope(ContractRegistry contractRegistry, Scope parentScope = null, string name = "scope")
        {
            Name = name;
            Contracts = contractRegistry ?? throw new ArgumentNullException(nameof(contractRegistry));
            _parentScope = parentScope;
            //_instanceEntries = new Dictionary<string, InstanceEntry>(8);
            //_nonReuseableEntries = new List<InstanceEntry>(8);
        }

        public ContractRegistry Contracts { get; }
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
            
        }

        public IScope CreateScope(string name = "scope")
        {
            return new Scope(Contracts, this);
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