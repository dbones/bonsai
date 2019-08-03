namespace Bonsai.LifeStyles
{
    using Contracts;
    using Exceptions;

    public class Singleton : ILifeSpan
    {
        private const string Name = "singleton";
        private IAdvancedScope _singletonScope;

        public object Resolve(IAdvancedScope currentScope, Contract contract, Contract parentContract)
        {
            var scope = GetNamedScope(currentScope, Name);
            if (scope.InstanceCache.TryGet(contract, out var entry)) return entry;

            lock (_singletonScope)
            {
                //check again.
                if (scope.InstanceCache.TryGet(contract, out entry)) return entry;

                //create new instance
                entry = contract.CreateInstance(currentScope, contract, parentContract);

                scope.InstanceCache.Add(contract, entry);
                scope.TrackInstance(contract, entry);
            }

            return entry;
        }

        IAdvancedScope GetNamedScope(IAdvancedScope scope, string name)
        {
            if (_singletonScope != null)
            {
                return _singletonScope;
            }

            if (scope.Name == name)
            {
                _singletonScope = scope;
                return scope;
            }

            if (scope.ParentScope == null)
            {
                throw new ScopeNotFoundException(name);
            }

            return GetNamedScope(scope.ParentScope, name);
        }
    }
}