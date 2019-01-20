namespace Bonsai.Types
{
    using System.Collections;
    using System.Collections.Generic;

    public class EnumerableService<T> : IEnumerable<T>
    {
        private readonly IAdvancedScope _scope;

        public EnumerableService(IAdvancedScope scope)
        {
            _scope = scope;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            var contracts = _scope.Contracts.GetAllContracts(typeof(T));
            foreach (var contract in contracts)
            {
                yield return (T)_scope.Resolve(contract);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}