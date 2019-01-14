namespace Bonsai.Types
{
    using System;

    public class LazyService<T> : Lazy<T>
    {
        private readonly IScope _scope;

        public LazyService(IScope scope)
        {
            _scope = scope;
        }
        
        
    }
}