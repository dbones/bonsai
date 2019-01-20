namespace Bonsai.LifeStyles
{
    using Contracts;
    using Internal;

    public class PerScope : ILifeSpan
    {
        public object Resolve(IAdvancedScope currentScope, Contract contract, Contract parentContract)
        {
            var entry = currentScope.InstanceCache.Get(contract.Id);

            if (entry == null)
            {
                entry = new Instance()
                {
                    Value = contract.CreateInstance(currentScope, contract, parentContract),
                    Contract = contract
                };
                currentScope.InstanceCache.Add(contract.Id, entry);
            }

            currentScope.TrackInstance(entry);

            return entry.Value;
        }
    }
}