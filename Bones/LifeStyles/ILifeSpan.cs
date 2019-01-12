namespace Bones
{
    using System;

    public interface ILifeSpan
    {
        object Resolve(IAdvancedScope currentScope, Contract contract);
    }
}