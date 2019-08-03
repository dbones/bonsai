namespace Bonsai.Internal
{
    using System;

    /// <summary>
    /// a unique key for a service
    /// </summary>
    public class ServiceKey : IEquatable<ServiceKey>
    {
        private readonly int _hash;
        public static readonly int DefaultNameHash = "default".GetHashCode();
        public static readonly string DefaultName = "default";

        public ServiceKey(Type service, string serviceName = null)
        {
            Service = service;

            //simple hashing, is the quickest we have atm, but its not full proof
            if (serviceName == null)
            {
                _hash = service.GetHashCode() * 31 + DefaultNameHash;
            }
            else
            {
                _hash = service.GetHashCode() * 31 + serviceName.GetHashCode();
                ServiceName = serviceName;
            }
        }

        public string ServiceName { get; }
        public Type Service { get; }

        public override string ToString()
        {
            return $"{Service.FullName} ^_^ {ServiceName ?? DefaultName}";
        }

        public bool Equals(ServiceKey other)
        {
            return _hash == other?.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return _hash == obj?.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hash;
        }
    }
}