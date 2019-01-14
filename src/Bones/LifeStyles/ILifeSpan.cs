namespace Bones.LifeStyles
{
    using Contracts;

    public interface ILifeSpan
    {
        object Resolve(IAdvancedScope currentScope, Contract contract);
    }
}