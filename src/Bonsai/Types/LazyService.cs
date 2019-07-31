namespace Bonsai.Types
{
    using System;
    using Contracts;
    using Internal;

    public class LazyService<T> : Lazy<T>
    {
        public LazyService(IAdvancedScope scope)
            : base(() =>
            {
                Code.Require(() => scope != null, nameof(scope));
                return scope.Resolve<T>();
            })
        {
        }
    }
    
}