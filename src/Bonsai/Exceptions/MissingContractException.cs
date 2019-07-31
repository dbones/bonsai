using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Bonsai.Exceptions
{
    using System;
    using Internal;

    public class MissingContractException : Exception
    {
        private readonly ServiceKey _key;

        public MissingContractException(ServiceKey key) : base($"cannot find contract for {key}")
        {
            _key = key;
        }
    }
}