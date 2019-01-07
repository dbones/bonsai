namespace Bones
{
    using Exceptions;

    public class Named : ILifeSpan
    {
        public string Name { get; set; }

        public object Resolve(Scope currentScope, Contract contract)
        {
            var scope = GetNamedScope(currentScope, Name);
            var entry = scope.InstanceCache.Get(contract.Id);

            if (entry == null)
            {
                entry = new Instance()
                {
                    Value = contract.CreateInstance(currentScope),
                    Contract = contract
                };

                scope.InstanceCache.Add(contract.Id, entry);
                scope.Tracked.Push(entry);
            }
 
            return entry.Value;
        }

        Scope GetNamedScope(Scope scope, string name)
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