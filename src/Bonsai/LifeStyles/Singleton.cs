namespace Bonsai.LifeStyles
{
    using Contracts;
    using Exceptions;
    using Internal;

    public class Singleton : ILifeSpan
    {
        private readonly string _name;
        private IAdvancedScope _singletonScope;
        
        public Singleton()
        {
            _name = "singleton";
        }
        
        public object Resolve(IAdvancedScope  currentScope, Contract contract, Contract parentContract)
        {
            var scope = GetNamedScope(currentScope, _name);
            var entry = scope.InstanceCache.Get(contract);

            if (entry != null) return entry.Value;
            
            lock (_singletonScope)
            {
                //check again.
                entry = scope.InstanceCache.Get(contract);
                if (entry != null) return entry.Value;
                
                //create new instance
                entry = new Instance()
                {
                    Value = contract.CreateInstance(currentScope, contract, parentContract),
                    Contract = contract
                };

                scope.InstanceCache.Add(contract, entry);
                scope.TrackInstance(entry);
            }

            return entry.Value;
        }

        IAdvancedScope  GetNamedScope(IAdvancedScope  scope, string name)
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