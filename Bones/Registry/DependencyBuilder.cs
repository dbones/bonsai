namespace Bones
{
    using System.Reflection;

    public class DependencyBuilder
    {
        private readonly RegistrationDependency _dependency;

        public DependencyBuilder(RegistrationDependency dependency)
        {
            _dependency = dependency;
        }

        public RegistrationDependency UseValue(object value)
        {
            _dependency.Value = value;
            return _dependency;
        }

        public RegistrationDependency Named(string name)
        {
            _dependency.Named = name;
            return _dependency;
        }

        public RegistrationDependency Use<T>(string named = null) where T : class
        {
            _dependency.ImplementedType = typeof(T).GetTypeInfo();
            _dependency.Named = named;
            return _dependency;
        }
    }
}