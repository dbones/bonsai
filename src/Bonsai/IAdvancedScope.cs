namespace Bonsai
{
    using Collections.Caching;
    using Contracts;
    using Internal;

    public interface IAdvancedScope : IScope
    {
        ContractRegistry Contracts { get; }
        
        ICache<Contract, Instance> InstanceCache { get; }

        void TrackInstance(Instance instance);

        string Name { get; }

        object Resolve(ServiceKey serviceKey);

        object Resolve(Contract contract, Contract parentContract = null);
        
        Scope ParentScope { get; }
    }
}