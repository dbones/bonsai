namespace Bonsai.LifeStyles
{
    using Contracts;
    using Exceptions;
    using Internal;

    public class Named : ILifeSpan
    {
        public string Name { private get; set; }        

        public object Resolve(IAdvancedScope  currentScope, Contract contract, Contract parentContract)
        {
            var scope = GetNamedScope(currentScope, Name);
            var entry = scope.InstanceCache.Get(contract);

            if (entry != null) return entry.Value;
            
            entry = new Instance
            {
                Value = contract.CreateInstance(currentScope, contract, parentContract),
                Contract = contract
            };

            scope.InstanceCache.Add(contract, entry);
            scope.TrackInstance(entry);

            return entry.Value;
        }

        IAdvancedScope  GetNamedScope(IAdvancedScope  scope, string name)
        {
            if (scope.Name == name)
            {
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