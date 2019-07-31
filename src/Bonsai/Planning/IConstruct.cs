namespace Bonsai.Planning
{
    using System.Collections.Generic;
    using Contracts;
    using RegistrationProcessing;

    public interface IConstruct
    {
        bool CanSupport(RegistrationContext context);
        CreateInstance Create(RegistrationContext context, IEnumerable<Contract> contracts);
    }
}