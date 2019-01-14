namespace Bonsai.Exceptions
{
    using System;

    public class ServiceDoesNotImplementContractException : Exception
    {
        public Type Contract { get; }
        public Type Service { get; }

        public ServiceDoesNotImplementContractException(Type contract, Type service)
        {
            Contract = contract;
            Service = service;
        }
    }
}