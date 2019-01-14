namespace Bonsai.Contracts
{
    using System.Collections.Generic;
    using Exceptions;
    using Internal;

    /// <summary>
    /// the collection of contracts, with some methods to find a contact
    /// </summary>
    public class ContractRegistry
    {
        private IDictionary<ServiceKey, Contract> _contracts;

        public ContractRegistry(IEnumerable<Contract> contracts)
        {
            _contracts = new Dictionary<ServiceKey, Contract>();
            foreach (var contract in contracts)
            {
                foreach (var key in contract.ServiceKeys)
                {
                    _contracts.Add(key, contract);
                }
            }
        }

        /// <summary>
        /// Get a contract by the service key
        /// </summary>
        /// <param name="serviceKey">the service key to search with</param>
        /// <returns></returns>
        /// <exception cref="ContractNotSupportedException">as it suggests</exception>
        public Contract GetContract(ServiceKey serviceKey)
        {
            //Code.Require(() => serviceKey != null, nameof(serviceKey));

            if (_contracts.TryGetValue(serviceKey, out var entry))
            {
                return entry;
            }

            throw new ContractNotSupportedException(serviceKey);
        }
    }
}