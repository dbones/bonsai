namespace Bones
{
    using System.Reflection;

    public class For
    {
        private RegistrationDependency _dependency = new RegistrationDependency();

        public DependencyBuilder Property(string name)
        {
            //_dependency.Type = PropertyInjectType.Instance;
            //_dependency.AttributeName = name;
            return new DependencyBuilder(_dependency);
        }

        public DependencyBuilder Contract<T>()
        {
            _dependency.RequiredType = typeof(T).GetTypeInfo();
            return new DependencyBuilder(_dependency);
        }

        public ParameterSelector Method(string name)
        {
            //_dependency.Type = MethodInjectType.Instance;
            _dependency.AttributeName = name;
            return new ParameterSelector(_dependency);
        }

        public ParameterSelector Constructor()
        {
            //_dependency.Type = ConstructorInjectType.Instance;
            return new ParameterSelector(_dependency);
        }
    }
}