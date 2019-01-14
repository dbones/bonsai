namespace Bones
{
    using System.Collections.Generic;
    using Collections;
    using Contracts;
    using Internal;

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
}