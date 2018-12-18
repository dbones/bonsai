namespace Bones
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using Exceptions;

    public class Scope
    {
        //private List<InstanceEntry> _nonReuseableEntries;
        //private IDictionary<string, InstanceEntry> _instanceEntries;
        private Scope _parentScope;

        public Scope(Scope parentScope)
        {
            _parentScope = parentScope;
            //_instanceEntries = new Dictionary<string, InstanceEntry>(8);
            //_nonReuseableEntries = new List<InstanceEntry>(8);
        }

        public string Name { get; set; }

        public object Resolve(Type contract)
        {
            throw new NotImplementedException("resolve");
        }

        public object Resolve(Type contract, string name)
        {
            //craete service Key
            
            //check all scopes depeding on the lifetime strategy
            
            //create instance
            
            throw new NotImplementedException();
        }
        
        
    }


    public interface ILifeScopeStrategy
    {
        
    }
    
    
    

//    public class ContractRegistry
//    {
//        IDictionary<ServiceKey, Contract> _serviceContracts = new Dictionary<ServiceKey, Contract>();
//        IDictionary<ServiceKey, Contract> _genericParameterContracts = new Dictionary<ServiceKey, Contract>();
//        private List<Contract> _contacts = new List<Contract>();
//
//        private object _lock = new object();
//        
//        public Contract Get(ServiceKey key)
//        {
//            Contract contract = null;
//            if (!key.Service.IsGenericType)
//            {
//                if (!_serviceContracts.TryGetValue(key, out contract))
//                {
//                    throw new MissingContractException(key);
//                }
//
//                return contract;
//            }
//
//            lock (_lock)
//            {
//                if (_genericParameterContracts.TryGetValue(key, out contract))
//                {
//                    return contract;
//                }
//
//                var genericContract = key.Service.GetGenericTypeDefinition();
//                //_contacts.FirstOrDefault(x=> x.Type == genericContract && x.Named == )
//                
//            }
//
//            return null;
//        }
//    }
//    
//    
//    /// <summary>
//    /// a unique key for a service
//    /// </summary>
//    public class ServiceKey
//    {
//        private readonly string _internalName;
//        private readonly int _hash;
//
//        public ServiceKey(Type service, string serviceName = "default")
//        {
//            ServiceName = serviceName;
//            Service = service;
//            _internalName = $"{Service.FullName} ^_^ {ServiceName}";
//            _hash = _internalName.GetHashCode();
//        }
//
//        public string ServiceName { get; protected set; }
//        public Type Service { get; protected set; }
//
//        public override string ToString()
//        {
//            return $"Key: {_internalName}";
//        }
//
//        public override bool Equals(object obj)
//        {
//            if (obj == null) return false;
//            return _hash == obj.GetHashCode();
//        }
//
//        public override int GetHashCode()
//        {
//            return _hash;
//        }
//    }
    
}