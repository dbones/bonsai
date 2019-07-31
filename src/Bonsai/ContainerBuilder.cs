namespace Bonsai
{
    using System.Collections.Generic;
    using System.Linq;
    using Collections.Caching;
    using Collections.LinkedLists;
    using Contracts;
    using Internal;
    using PreContainer;
    using PreContainer.RegistrationProcessing;
    using Registry;

    /// <summary>
    /// use this to setup and create a container.
    /// </summary>
    public class ContainerBuilder
    {
        private readonly List<Registration> _registrations = new List<Registration>();

        public ContainerBuilder()
        {
            this.SetupModules(new InternalModule());
        }

        /// <summary>
        /// setup the container via many modules.
        /// </summary>
        /// <param name="modules">all the modules which will configure the container</param>
        public void SetupModules(params IModule[] modules)
        {
            Code.Require(() => modules != null, nameof(modules));

            foreach (var module in modules)
            {
                module.Setup(this);
            }
        }

        /// <summary>
        /// create an instance of the container.
        /// </summary>
        public IContainer Create()
        {
            //find known dependencies
            //create contracts for all contracts
            //allow for any modifications
            //build delegates
            //build contract registry
            //build initial scope and return as container.

            var registrationRegistry = new RegistrationRegistry(_registrations);
            new InjectionPlanner(registrationRegistry).Plan();

            //make this reusable
            var contractRegistry = new ContractRegistry(registrationRegistry);

            return new Scope(
                contractRegistry, 
                null, 
                "singleton",
                new ConcurrentLinkedList<Instance>(),
                new ConcurrentAvlCache<Contract, Instance>());
        }

        /// <summary>
        /// register a contract (well the registration which will be computed into a contract) with the container, this is how the IoC will know what depends on what
        /// </summary>
        /// <param name="registration">the registration</param>
        public void RegisterContract(Registration registration)
        {
            _registrations.Add(registration);
        }
    }


    public class DependencySetupStrategy
    {
        RegistrationScanner registrationScanner;
        DelegateBuilder deletBuilder = new DelegateBuilder();
        ContractRegistry contractRegistry;


        public DependencySetupStrategy(RegistrationRegistry registrationRegistry)
        {
            registrationScanner = new RegistrationScanner(registrationRegistry);
        }


        public IEnumerable<Contract> HandleInitialSetup()
        {
            var contexts = registrationScanner.CreateContexts().ToList();
            var contracts = CreateContracts(contexts);

            foreach (var contract in contracts)
            {
                deletBuilder.SetDelegates(contexts, contracts, contract);
            }

            return contracts;
        }


        public IEnumerable<Contract> HandleContractSetup(ServiceKey key, IEnumerable<Contract> allContracts)
        {
            var contexts = registrationScanner.CreateContexts(key).ToList();
            var contracts = CreateContracts(contexts);

            foreach (var contract in contracts)
            {
                deletBuilder.SetDelegates(registrationScanner.RegistrationContexts, contracts.Union(allContracts).ToList(), contract);
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