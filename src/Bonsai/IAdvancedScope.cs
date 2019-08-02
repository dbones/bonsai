namespace Bonsai
{
    using Collections.Caching;
    using Contracts;
    using Internal;

    public interface IAdvancedScope : IScope
    {
        ContractRegistry Contracts { get; }
        
        ICache<Contract, object> InstanceCache { get; }

        void TrackInstance(Contract contract, object instance);

        string Name { get; }

        object Resolve(ServiceKey serviceKey);

        object Resolve(Contract contract, Contract parentContract = null);
        
        Scope ParentScope { get; }
    }
}