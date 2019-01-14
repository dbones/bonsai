namespace Bones.PreContainer
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Internal;
    using Registry;

    public class RegistrationRegistry : IEnumerable<Registration>
    {
        private List<Registration> _registrations;

        public RegistrationRegistry(IEnumerable<Registration> registrations)
        {
            Code.Require(() => registrations != null, nameof(registrations));
            _registrations = registrations.ToList();
        }


        public bool Contains(ServiceKey key)
        {
            if (key == null) return false;
            if (_registrations.Any(x => x.Types.Contains(key)))
            {
                return true;
            }

            if (!key.Service.IsGenericType)
            {
                return false;
            }

            var t = key.Service.GetGenericTypeDefinition();
            return _registrations.Any(x =>
                x.Types.Any(supported => supported.Service == t && supported.ServiceName == key.ServiceName));
        }


        public Registration BySupportingType(ServiceKey exposedType)
        {
            Code.Require(() => exposedType != null, nameof(exposedType));

            return _registrations.FirstOrDefault(x =>
            {
                if (x.Types.Contains(exposedType))
                {
                    return true;
                }


                if (!exposedType.Service.IsGenericType)
                {
                    return false;
                }

                return x.Types.Any(possibleType =>
                    possibleType.Service.IsGenericType &&
                    possibleType.Service.GetGenericTypeDefinition() == exposedType.Service.GetGenericTypeDefinition());
            });
        }


        public IEnumerator<Registration> GetEnumerator()
        {
            return _registrations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}