namespace Bones
{
    public class Provided : ILifeSpan
    {
        public object Resolve(Scope currentScope, Contract contract)
        {
            return contract.Instance;
        }
    }
}