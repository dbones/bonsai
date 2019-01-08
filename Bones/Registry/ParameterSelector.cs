namespace Bones
{
    using System.Reflection;

    public class ParameterSelector
    {
        private readonly RegistrationDependency _dependency;

        public ParameterSelector(RegistrationDependency dependency)
        {
            _dependency = dependency;
        }

        public DependencyBuilder Parameter(string name)
        {
            Code.Require(() => !string.IsNullOrEmpty(name), nameof(name));

            _dependency.ParameterPredicates.Add(parameter => parameter.Name == name);
            _dependency.ParameterName = name;
            return new DependencyBuilder(_dependency);
        }

        public DependencyBuilder ParameterWithType<T>()
        {
            _dependency.ParameterPredicates.Add(parameter => parameter.ParameterType == typeof(T));
            _dependency.RequiredType = typeof(T).GetTypeInfo();
            return new DependencyBuilder(_dependency);
        }
    }
}