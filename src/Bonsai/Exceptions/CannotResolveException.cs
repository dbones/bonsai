namespace Bonsai.Exceptions
{
    using System;
    using Contracts;

    public class CannotResolveException : Exception
    {
        public CannotResolveException(Contract contract, Exception innerException) : base(contract.ToString(), innerException)
        {
        }
    }
}