namespace Bonsai.LifeStyles
{
    using System.Runtime.InteropServices.ComTypes;
    using Contracts;

    public class PerScope : ILifeSpan
    {
        public object Resolve(IAdvancedScope currentScope, Contract contract, Contract parentContract)
        {
            if (currentScope.InstanceCache.TryGet(contract, out var entry)) return entry;
            
            entry = contract.CreateInstance(currentScope, contract, parentContract);
            
            currentScope.InstanceCache.Add(contract, entry);
            currentScope.TrackInstance(contract, entry);

            return entry;
        }
    }
}