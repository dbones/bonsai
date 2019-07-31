namespace Bonsai.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using ImTools;
    using Internal;

    /// <summary>
    /// the collection of contracts, with some methods to find a contact
    /// </summary>
    public class ContractRegistry
    {
        private ImHashMap<ServiceKey, Contract> _contracts;
        private ImHashMap<Type, List<Contract>> _contractsByType;
        private volatile int _counter = 0;

        public ContractRegistry(IEnumerable<Contract> contracts)
        {
            _contractsByType = ImHashMap<Type, List<Contract>>.Empty;
            _contracts = ImHashMap<ServiceKey, Contract>.Empty;
            foreach (var contract in contracts)
            {
                AddContract(contract);
            }
        }

        public void AddContract(Contract contract)
        {
            foreach (var key in contract.ServiceKeys)
            {
                if (key.ServiceName == "default" && _contracts.TryFind(key, out var oldDefault))
                {
                    _counter++;
                    _contracts = _contracts.Update(key, contract);
                    var renamedKey = new ServiceKey(key.Service, $"{key.ServiceName}-{_counter}");
                    _contracts = _contracts.AddOrUpdate(renamedKey, oldDefault);
                }
                else
                {
                    _contracts = _contracts.AddOrUpdate(key, contract);
                }

                //list lookup
                if (!_contractsByType.TryFind(key.Service, out var contractsForType))
                {
                    contractsForType = new List<Contract>();
                    _contractsByType = _contractsByType.AddOrUpdate(key.Service, contractsForType);
                }

                if (!contractsForType.Contains(contract))
                {
                    contractsForType.Add(contract);
                }
            }

            //scope lookup
            if (contract.ServiceKeys.Any(x => typeof(IScope).IsAssignableFrom(x.Service)))
            {
                ScopeContract = contract;
            }
        }

        public Contract ScopeContract { get; private set; }

        public IEnumerable<Contract> GetAllContracts(Type type)
        {
            return _contractsByType.TryFind(type, out var result)
                ? result
                : Enumerable.Empty<Contract>();
        }

        /// <summary>
        /// Get a contract by the service key
        /// </summary>
        /// <param name="serviceKey">the service key to search with</param>
        /// <returns></returns>
        /// <exception cref="ContractNotSupportedException">as it suggests</exception>
        public Contract GetContract(ServiceKey serviceKey)
        {
            if (_contracts.TryFind(serviceKey, out var entry))
            {
                return entry;
            }

            throw new ContractNotSupportedException(serviceKey);
        }
    }
}