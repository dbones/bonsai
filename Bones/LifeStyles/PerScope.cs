namespace Bones
{
    public class PerScope : ILifeSpan
    {
        public object Resolve(IAdvancedScope currentScope, Contract contract)
        {
            var entry = currentScope.InstanceCache.Get(contract.Id);

            if (entry == null)
            {
                entry = new Instance()
                {
                    Value = contract.CreateInstance(currentScope),
                    Contract = contract
                };
                currentScope.InstanceCache.Add(contract.Id, entry);
            }

            currentScope.Tracked.Push(entry);

            return entry.Value;
        }
    }
}