namespace Bonsai.Exceptions
{
    using System;
    using Internal;

    public class ContractNotSupportedException : Exception
    {
        public ServiceKey Key { get; }
        public ContractNotSupportedException(ServiceKey serviceKey) : base(serviceKey.ToString())
        {
            Key = serviceKey;
        }
    }
}