namespace Bonsai.Planning
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Internal;
    using RegistrationProcessing;

    public class DependencySetupStrategy
    {
        readonly RegistrationScanner _registrationScanner;
        readonly DelegateBuilder _delegateBuilder = new DelegateBuilder();


        public DependencySetupStrategy(RegistrationRegistry registrationRegistry)
        {
            _registrationScanner = new RegistrationScanner(registrationRegistry);
        }


        public IEnumerable<Contract> HandleInitialSetup()
        {
            var contexts = _registrationScanner.CreateContexts().ToList();
            var contracts = CreateContracts(contexts);

            foreach (var contract in contracts)
            {
                _delegateBuilder.SetDelegates(contexts, contracts, contract);
            }

            return contracts;
        }


        public IEnumerable<Contract> HandleContractSetup(ServiceKey key, IEnumerable<Contract> allContracts)
        {
            var contexts = _registrationScanner.CreateContexts(key).ToList();
            var contracts = CreateContracts(contexts);

            foreach (var contract in contracts)
            {
                _delegateBuilder.SetDelegates(_registrationScanner.RegistrationContexts, contracts.Union(allContracts).ToList(), contract);
            }

            return contracts;
        }


        private ICollection<Contract> CreateContracts(IEnumerable<RegistrationContext> contexts)
        {
            var contracts = new List<Contract>();
            foreach (var context in contexts)
            {
                var contract = new Contract(context.Id)
                {
                    LifeSpan = context.Registration.ScopedTo,
                    ServiceKeys = context.Keys,
                    CreateInstance = context.Registration.CreateInstance,
                    Instance = context.Registration.Instance
                };

                contracts.Add(contract);
            }

            return contracts;
        }
    }
}