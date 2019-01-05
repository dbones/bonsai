namespace Bones
{
    public class Transient : ILifeSpan
    {
        public object Resolve(Scope currentScope, Contract contract)
        {
            return contract.CreateInstance(currentScope);
        }
    }
}