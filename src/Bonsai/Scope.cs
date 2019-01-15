namespace Bonsai
{
    using System;
    using System.Collections.Generic;
    using Collections;
    using Contracts;
    using Internal;


    public class Scope : IAdvancedScope
    {
        private readonly Collections.LinkedList<Instance> _tracked;
        
        public Scope(ContractRegistry contractRegistry, Scope parentScope = null, string name = "scope")
        {
            Name = name;
            Contracts = contractRegistry ?? throw new ArgumentNullException(nameof(contractRegistry));
            ParentScope = parentScope;
            InstanceCache = new SimpleCache<string, Instance>(5);
            _tracked = new Collections.LinkedList<Instance>();
        }

        public ICache<string, Instance> InstanceCache { get; }

        public void TrackInstance(Instance instance)
        {
            _tracked.AddNode(instance);
        }
        
        public ContractRegistry Contracts { get; }
        public Scope ParentScope { get; }
        public string Name { get; }

        public object Resolve(ServiceKey serviceKey)
        {
            //Code.Require(()=> serviceKey != null, nameof(serviceKey));

            var contract = Contracts.GetContract(serviceKey);
            return contract.LifeSpan.Resolve(this, contract);
        }
        
        public object Resolve(Contract contract)
        {
            //Code.Require(()=> contract != null, nameof(contract));
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
            foreach (var instance in _tracked.GetItems())
            {
                instance.Contract.DisposeInstance(instance.Value);
            }
        }

        public IScope CreateScope(string name = "scope")
        {
            return new Scope(Contracts, this, name);
        }
    }
}