namespace Bonsai.LifeStyles
{
    using Contracts;
    using Internal;

    public class Transient : ILifeSpan
    {
        public object Resolve(IAdvancedScope currentScope, Contract contract, Contract parentContract)
        {
            var instance = new Instance()
            {
                Value = contract.CreateInstance(currentScope, contract, parentContract),
                Contract = contract
            };
            
            currentScope.TrackInstance(instance);
            
            return instance.Value;
        }
        
        
    }
}