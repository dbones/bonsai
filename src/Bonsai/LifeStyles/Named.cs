namespace Bonsai.LifeStyles
{
    using Contracts;
    using Exceptions;

    public class Named : ILifeSpan
    {
        public string Name { private get; set; }

        public object Resolve(IAdvancedScope currentScope, Contract contract, Contract parentContract)
        {
            var scope = GetNamedScope(currentScope, Name);
            if (scope.InstanceCache.TryGet(contract, out var entry)) return entry;

            entry =  contract.CreateInstance(currentScope, contract, parentContract);

            scope.InstanceCache.Add(contract, entry);
            scope.TrackInstance(contract, entry);

            return entry;
        }

        IAdvancedScope GetNamedScope(IAdvancedScope scope, string name)
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