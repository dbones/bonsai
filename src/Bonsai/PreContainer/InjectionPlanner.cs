namespace Bonsai.PreContainer
{
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using Internal;
    using RegistrationProcesing;
    using Registry;

    public class InjectionPlanner
    {
        private readonly RegistrationRegistry _registrations;
        private readonly ConstructorPlanner _constructorPlanner;

        public InjectionPlanner(RegistrationRegistry registrations)
        {
            _registrations = registrations;
            _constructorPlanner = new ConstructorPlanner(registrations);
        }

        public void Plan()
        {
            foreach (var registration in _registrations)
            {
                _constructorPlanner.Plan(registration);
            }
        }

        public class ConstructorPlanner
        {
            private readonly RegistrationRegistry _registrations;

            public ConstructorPlanner(RegistrationRegistry registrations)
            {
                _registrations = registrations;
            }

            public void Plan(Registration registration)
            {
                if (registration.Constructor != null 
                    || registration.Instance != null 
                    || registration.ImplementedType == typeof(Scope))
                {
                    return;
                }

                var constructors =
                    registration.ImplementedType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

                registration.Constructor = constructors
                    .Select(x => new {Score = Score(x, registration), Constructor = x})
                    .Where(x => x.Score >= 0)
                    .OrderByDescending(x => x.Score)
                    .Select(x => x.Constructor)
                    .FirstOrDefault();

                Code.Ensure(
                    () => registration.Constructor != null,
                    () => new CannotFindSupportableConstructorException(registration.ImplementedType));
            }

            int Score(MethodBase method, Registration registration)
            {
                Code.Require(() => method != null, nameof(method));
                Code.Require(() => registration != null, nameof(registration));

                var parameters = method.GetParameters();

                var plannedMethodParameters = registration.Dependencies.Where(d =>
                        d.MethodPredicates.Count == 0
                        || d.MethodPredicates.All(pred => pred(method)))
                    .ToList();

                int extraPoints = 0;

                var count = parameters.Count(parameter =>
                {
                    //see if it is a dependency which the user has provided
                    var planned = plannedMethodParameters
                        .Select(plannedDepdencency => new
                        {
                            Count = plannedDepdencency.ParameterPredicates.Count(p => p(parameter)),
                            Dependency = plannedDepdencency
                        })
                        .OrderByDescending(x => x.Count)
                        .Where(x => x.Count > 0)
                        .Select(x => x.Dependency)
                        .FirstOrDefault();

                    bool ContainsPlannedType()
                    {
                        if (planned == null) return false;

                        var name = planned.Named ?? "default";
                        var type = planned.RequiredType ?? parameter.ParameterType;
                        var key = new ServiceKey(type, name);

                        return _registrations.BySupportingType(key) != null;
                    }


                    if (planned?.Value != null 
                        || planned?.CreateInstance != null 
                        || ContainsPlannedType())
                    {
                        extraPoints += 5;
                        return true;
                    }

                    return _registrations.Contains(new ServiceKey(parameter.ParameterType));
                });

                if (count != parameters.Count())
                {
                    //not supported.
                    return -1;
                }

                return count + extraPoints;
            }
        }
    }
}