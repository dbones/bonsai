namespace Bonsai.LifeStyles
{
    using Contracts;
    using Internal;

    public class Transient : ILifeSpan
    {
        public object Resolve(IAdvancedScope currentScope, Contract contract, Contract parentContract)
        {
            var value = contract.CreateInstance(currentScope, contract, parentContract);

            if (contract.IsDisposal)
            {
                var instance = new Instance
                {
                    Value = value,
                    Contract = contract
                };
                currentScope.TrackInstance(instance);
            }

            return value;
        }
    }
}