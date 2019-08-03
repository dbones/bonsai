namespace Bonsai
{
    using System;
    using Collections.Caching;
    using Collections.LinkedLists;
    using Contracts;
    using Internal;

    public class Scope : IAdvancedScope
    {
        private readonly ILinkedList<object> _tracked;
        private volatile bool _isDisposing = false;

        public Scope(
            ContractRegistry contractRegistry,
            Scope parentScope = null,
            string name = "scope",
            ILinkedList<object> trackingCollection = null,
            ICache<Contract, object> instanceCache = null)
        {
            Contracts = contractRegistry ?? throw new ArgumentNullException(nameof(contractRegistry));
            ParentScope = parentScope;
            Name = name;
            _tracked = trackingCollection ?? new LinkedList<object>();
            InstanceCache = instanceCache ?? new SimpleCache<Contract, object>(5);

            InstanceCache.Add(Contracts.ScopeContract, this);
        }

        public ICache<Contract, object> InstanceCache { get; }
        public ContractRegistry Contracts { get; }

        public Scope ParentScope { get; }
        public string Name { get; }

        public void TrackInstance(Contract contract, object instance)
        {
            if (contract.IsDisposal)
            {
                _tracked.Add(instance);                
            }
        }

        public object Resolve(Contract contract, Contract parentContract = null)
        {
            return contract.LifeSpan.Resolve(this, contract, parentContract);
        }

        public object Resolve(ServiceKey serviceKey)
        {
            var contract = Contracts.GetContract(serviceKey);
            return Resolve(contract);
        }

        public TService Resolve<TService>(string serviceName = null)
        {
            return (TService) Resolve(new ServiceKey(typeof(TService), serviceName));
        }

        public object Resolve(Type service, string name = null)
        {
            Code.Require(() => service != null, nameof(service));
            return Resolve(new ServiceKey(service, name));
        }


        void IDisposable.Dispose()
        {
            if (_isDisposing) return;
            _isDisposing = true;

            foreach (var instance in _tracked.GetAll())
            {
                ((IDisposable) instance)?.Dispose();
            }
        }

        public IScope CreateScope(string name = "scope")
        {
            return new Scope(Contracts, this, name);
        }
    }


}