namespace Bonsai.LifeStyles
{
    using Contracts;

    public class Transient : ILifeSpan
    {
        public object Resolve(IAdvancedScope currentScope, Contract contract, Contract parentContract)
        {
            var value = contract.CreateInstance(currentScope, contract, parentContract);

            currentScope.TrackInstance(contract, value);
            return value;
        }
    }
}