namespace Bonsai
{
    using System.Security.Cryptography.X509Certificates;
    using Collections;
    using Collections.Caching;
    using Contracts;
    using Internal;

    public interface IAdvancedScope : IScope
    {
        ContractRegistry Contracts { get; }
        
        ICache<string, Instance> InstanceCache { get; }

        void TrackInstance(Instance instance);

        string Name { get; }

        object Resolve(ServiceKey serviceKey);

        object Resolve(Contract contract, Contract parentContract = null);
        
        Scope ParentScope { get; }
    }
}