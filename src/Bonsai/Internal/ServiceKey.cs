namespace Bonsai.Internal
{
    using System;

    /// <summary>
    /// a unique key for a service
    /// </summary>
    public class ServiceKey
    {
        private readonly string _internalName;
        private readonly int _hash;

        public ServiceKey(Type service, string serviceName = "default")
        {
            Code.Require(()=> service != null, nameof(service));
            
            ServiceName = serviceName;
            Service = service;
            _internalName = $"{Service.FullName} ^_^ {ServiceName}";
            _hash = _internalName.GetHashCode();
        }

        public string ServiceName { get; protected set; }
        public Type Service { get; protected set; }

        public override string ToString()
        {
            return $"Key: {_internalName}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return _hash == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hash;
        }
    }
}