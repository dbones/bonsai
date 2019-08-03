namespace Bonsai.Planning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Contracts;
    using RegistrationProcessing;

    public class DelegateBuilder
    {
        public List<IConstruct> _constructs = new List<IConstruct>() { new IlConstruct(), new FuncConstruct() };

        public void SetDelegates(ICollection<RegistrationContext> contexts, ICollection<Contract> contracts, Contract contract)
        {
            var context = contexts.First(x => x.Id == contract.Id);

            if (context.Registration.Instance != null)
            {
                //provided instance
                //do nothing
            }
            else if (context.Registration.CreateInstance != null)
            {
                //provided a ctor
                contract.CreateInstance = context.Registration.CreateInstance;
            }
            else
            {
                //need to create a delegate
                contract.CreateInstance = _constructs.First(x => x.CanSupport(context)).Create(context, contracts);
            }

            contract.IsDisposal = typeof(IDisposable).IsAssignableFrom(context.ImplementedType);
        }


    }
}