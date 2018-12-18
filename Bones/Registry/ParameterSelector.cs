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
            _dependency.ParameterName = name;
            return new DependencyBuilder(_dependency);
        }

        public DependencyBuilder ParameterWithType<T>()
        {
            _dependency.RequiredType = typeof(T).GetTypeInfo();
            return new DependencyBuilder(_dependency);
        }
    }
}