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

        public void UseValue(object value)
        {
            _dependency.Value = value;
        }

        /// <summary>
        /// the named dependency to associate with this
        /// </summary>
        public void Named(string name)
        {
            _dependency.Named = name;
        }

        public void Use<T>(string named = null) where T : class
        {
            _dependency.ImplementedType = typeof(T).GetTypeInfo();
            _dependency.Named = named;
        }
    }
}