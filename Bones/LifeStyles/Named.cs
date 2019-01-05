namespace Bones
{
    public class Named : ILifeSpan
    {
        public string Name { get; set; }
        public object Resolve(Scope currentScope, Contract contract)
        {
            throw new System.NotImplementedException();
        }
    }
}