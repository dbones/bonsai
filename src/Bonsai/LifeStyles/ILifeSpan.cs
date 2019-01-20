namespace Bonsai.LifeStyles
{
    using Contracts;
    using Internal;

    public interface ILifeSpan
    {
        object Resolve(IAdvancedScope currentScope, Contract contract, Contract parentContract = null);
    }
}