namespace Bones
{
    public interface ILifeSpan
    {
        object Resolve(Scope currentScope, Contract contract);
    }
}