namespace Bones
{
    using System;

    public interface ILifeSpan
    {
        object Resolve(Scope currentScope, Contract contract);
    }
}