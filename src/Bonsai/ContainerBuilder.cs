namespace Bonsai
{
    using System.Collections.Generic;
    using Collections.Caching;
    using Collections.LinkedLists;
    using Contracts;
    using Internal;
    using Planning;
    using Planning.RegistrationProcessing;
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
                new ConcurrentLinkedList<object>(),
                new ConcurrentAvlCache<Contract, object>());
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
}