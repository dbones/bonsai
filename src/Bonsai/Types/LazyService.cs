namespace Bonsai.Types
{
    using System;
    using Contracts;

    public class LazyService<T> : Lazy<T>
    {
        public LazyService(IAdvancedScope scope, Contract contract)
            : base(() => scope.Resolve<T>())
        {
        }
    }
}