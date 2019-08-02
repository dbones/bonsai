namespace Bonsai.Planning.RegistrationProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internal;
    using Registry;

    public class RegistrationScanner
    {
        private readonly RegistrationRegistry _registrations;
        private int _counter = 0;

        private readonly HashSet<string> _processedContextHashes = new HashSet<string>();
        public ICollection<RegistrationContext> RegistrationContexts { get; set; }

        public RegistrationScanner(RegistrationRegistry registrations)
        {
            _registrations = registrations;
            RegistrationContexts = new List<RegistrationContext>();
        }

        /// <summary>
        /// this is used to find all known contracts at the start of the container build
        /// </summary>
        public ICollection<RegistrationContext> CreateContexts()
        {
            var results = new List<RegistrationContext>();

            foreach (var registration in _registrations
                .Where(x => !x.ImplementedType.IsGenericTypeDefinition))
            {
                GetRegistrationContext(registration, results);
            }

            return results;
        }

        /// <summary>
        /// this is to find new contexts for the unknown uses of generic types
        /// where the container is not being used directly, but is being used inside a factory class
        /// </summary>
        public IEnumerable<RegistrationContext> CreateContexts(ServiceKey key)
        {
            var registration = _registrations.BySupportingType(key);
            var results = new List<RegistrationContext>();

            if (registration == null)
            {
                return Enumerable.Empty<RegistrationContext>();
            }
            
            GetRegistrationContext(registration, results, key.Service);
            return results;
        }


        /// <summary>
        /// this looks over the code base, trying to find all known generic types and types
        /// </summary>
        /// <remarks>
        /// this is probably the most complex code in this codebase, we need to refactor to make cleaner
        /// </remarks>
        private void GetRegistrationContext(
            Registration registration,
            List<RegistrationContext> foundContexts,
            Type registrationType = null)
        {
            Code.Require(() => registration != null, nameof(registration));

            string hash;
            hash = registrationType?.IsGenericType == true && registration.ImplementedType.IsGenericTypeDefinition == true
                ? $"{registration.Id} {registration.ImplementedType.MakeGenericType(registrationType.GenericTypeArguments)}"
                : $"{registration.Id} {registration.ImplementedType.FullName}";

            //already processed
            var haveRegistration = _processedContextHashes.Contains(hash);
            if (haveRegistration)
            {
                return;
            }

            _counter++;
            RegistrationContext context = new RegistrationContext(_counter);
            context.Registration = registration;
            _processedContextHashes.Add(hash);
            foundContexts.Add(context);
            RegistrationContexts.Add(context);
            
            //object provided or delegate provided
            if (registration.Instance != null || registration.CreateInstance != null)
            {
                context.Keys = registration.Types;
                return;
            }
            

            var constructor = new MethodInformation()
            {
                InjectOn = InjectOn.Constructor,
                Name = "ctor"
            };
            context.InjectOnMethods.Add(constructor);


            //get the actual constructor
            if (registrationType?.IsGenericType == true)
            {
                var ctorParams = registration.Constructor.GetParameters();

                var type = registration.ImplementedType.MakeGenericType(registrationType.GenericTypeArguments);
                var c = type.GetConstructors()
                    .Where(x => x.GetParameters().Length == ctorParams.Length)
                    .Where(x =>
                    {
                        var candidateParams = x.GetParameters();
                        for (int i = 0; i < candidateParams.Length; i++)
                        {
                            var parameter = ctorParams[i].ParameterType;
                            var candidateParameter = candidateParams[i].ParameterType;

                            if (parameter == candidateParameter)
                            {
                                continue;
                            }

                            if (parameter.IsGenericType && candidateParameter.IsGenericType
                                                        && parameter.GetGenericTypeDefinition() ==
                                                        candidateParameter.GetGenericTypeDefinition())
                            {
                                continue;
                            }

                            return false;
                        }

                        return true;
                    }).FirstOrDefault();

                constructor.Method = c;
                context.ImplementedType = type;
                context.Keys = new HashSet<ServiceKey>( 
                    registration.Types.Select(t => t.Service.IsGenericTypeDefinition 
                    ? new ServiceKey(t.Service.MakeGenericType(type.GenericTypeArguments), t.ServiceName) 
                    : t));
            }
            else
            {
                constructor.Method = (ConstructorInfo) registration.Constructor;
                context.ImplementedType = registration.ImplementedType;
                context.Keys = registration.Types;
            }

            var parameters = constructor.Method.GetParameters();
            foreach (var parameter in parameters)
            {
                var dependency =
                    registration.Dependencies.FirstOrDefault(x =>
                        x.ParameterPredicates.All(pred => pred(parameter))
                        && x.InjectOn == InjectOn.Constructor);
                
                if (dependency?.Value != null)
                {
                    constructor.Parameters.Add(new ParameterInformation()
                    {
                        Name = parameter.Name,
                        Value = dependency.Value
                    });
                    continue;
                }

                if (dependency?.CreateInstance != null)
                {
                    constructor.Parameters.Add(new ParameterInformation()
                    {
                        Name = parameter.Name,
                        ProvidedType = dependency.RequiredType,
                        CreateInstance = dependency.CreateInstance
                    });
                    continue;
                }

                var type = dependency?.RequiredType ?? parameter.ParameterType;
                var name = dependency?.Named;

                var dependencyKey = new ServiceKey(type, name);

                constructor.Parameters.Add(new ParameterInformation()
                {
                    Name = parameter.Name,
                    ServiceKey = dependencyKey
                });

                //recurive search
                var dependencyRegistration = _registrations.BySupportingType(dependencyKey);
                GetRegistrationContext(dependencyRegistration, foundContexts, type);
            }
        }
    }
}