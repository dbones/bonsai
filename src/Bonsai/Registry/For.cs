namespace Bonsai.Registry
{
    using System.Reflection;
    using Internal;

    public class For
    {
        private RegistrationDependency _dependency;

        public For(RegistrationDependency dependency)
        {
            _dependency = dependency;
        }

        public DependencyBuilder Property(string name)
        {
            Code.Require(() => !string.IsNullOrEmpty(name), nameof(name));

            _dependency.InjectOn = InjectOn.Constructor;
            _dependency.AttributeName = name;

            _dependency.MethodPredicates.Add(method => 
                method.MemberType  == MemberTypes.Property
                && method.Name == name);

            return new DependencyBuilder(_dependency);
        }

        public DependencyBuilder Contract<T>()
        {
            _dependency.RequiredType = typeof(T);

            _dependency.ParameterPredicates.Add(parameter => parameter.ParameterType == _dependency.RequiredType);
            

            return new DependencyBuilder(_dependency);
        }

        public ParameterSelector Method(string name)
        {
            Code.Require(() => !string.IsNullOrEmpty(name), nameof(name));

            _dependency.MethodPredicates.Add(method => 
                method.MemberType  == MemberTypes.Method 
                && method.Name == name);

            _dependency.InjectOn = InjectOn.Method;
            _dependency.AttributeName = name;
            return new ParameterSelector(_dependency);
        }

        public ParameterSelector Constructor()
        {
            _dependency.InjectOn = InjectOn.Constructor;
            _dependency.MethodPredicates.Add(method => method.MemberType  == MemberTypes.Constructor);

            return new ParameterSelector(_dependency);
        }
    }
}