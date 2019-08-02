namespace Bonsai.Internal
{
    using System;

    /// <summary>
    /// a unique key for a service
    /// </summary>
    public class ServiceKey : IEquatable<ServiceKey>
    {
        private readonly int _hash;
        public static int _defaultNameHash = "default".GetHashCode();
        public static string _defaultName = "default";

        public ServiceKey(Type service, string serviceName = null)
        {
            ServiceName = serviceName;
            Service = service;

            //simple hashing, is the quickest we have atm, but its not full proof
            _hash = serviceName == null 
                ? service.GetHashCode() * 31 + _defaultNameHash 
                : service.GetHashCode() * 31 + serviceName.GetHashCode();
        }

        public string ServiceName { get; }
        public Type Service { get; }

        public override string ToString()
        {
            return $"{Service.FullName} ^_^ {ServiceName ?? _defaultName}";
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