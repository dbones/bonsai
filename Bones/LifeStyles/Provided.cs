namespace Bones
{
    public class Provided : ILifeSpan
    {
        public object Resolve(IAdvancedScope currentScope, Contract contract)
        {
            return contract.Instance;
        }
    }
}