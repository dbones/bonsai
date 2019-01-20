namespace Bonsai.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Exceptions;
    using Internal;

    /// <summary>
    /// the collection of contracts, with some methods to find a contact
    /// </summary>
    public class ContractRegistry
    {
        private IDictionary<ServiceKey, Contract> _contracts;
        private IDictionary<Type, List<Contract>> _contractsByType;

        public ContractRegistry(IEnumerable<Contract> contracts)
        {
            _contractsByType = new Dictionary<Type, List<Contract>>();
            _contracts = new Dictionary<ServiceKey, Contract>();
            foreach (var contract in contracts)
            {
                foreach (var key in contract.ServiceKeys)
                {
                    //main lookup
                    _contracts.Add(key, contract);

                    //list lookup
                    if (!_contractsByType.TryGetValue(key.Service, out var contractsForType))
                    {
                        contractsForType = new List<Contract>();
                        _contractsByType.Add(key.Service, contractsForType);
                    }

                    if (!contractsForType.Contains(contract))
                    {
                        contractsForType.Add(contract);
                    }
                }
                
                //scope lookup
                if (contract.ServiceKeys.Any(x=> typeof(IScope).IsAssignableFrom(x.Service)))
                {
                    ScopeContract = contract;
                }
            }
        }

        public Contract ScopeContract { get; }

        public IEnumerable<Contract> GetAllContracts(Type type)
        {
            return _contractsByType[type];
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