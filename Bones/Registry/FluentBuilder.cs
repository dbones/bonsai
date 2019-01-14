namespace Bones.Registry
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using Internal;
    using LifeStyles;

    public class FluentBuilder
    {
        protected internal readonly Registration Registration;

        public FluentBuilder(Registration registration, Type serviceType)
        {
            this.Registration = registration;
            Registration.ImplementedType = serviceType.GetTypeInfo();
        }

        public FluentBuilder As<TContract>(string name = "default") where TContract : class
        {
            var contract = typeof(TContract).GetTypeInfo();
            if (!contract.IsAssignableFrom(Registration.ImplementedType))
            {
                //TODO: Exceptions
            }

            Registration.Types.Add(new ServiceKey(typeof(TContract).GetTypeInfo(), name));
            return this;
        }
        
        public FluentBuilder As(Type serviceType, string name = "default") 
        {
            var contract = serviceType.GetTypeInfo();
            if (!contract.IsAssignableFrom(Registration.ImplementedType))
            {
                //TODO: Exceptions
            }

            Registration.Types.Add(new ServiceKey(contract, name));
            return this;
        }


//        public FluentBuilder ConstructUsing(Func<ResolvingContext, object> ctor)
//        {
//            Registration.Constructor = ctor ?? throw new ArgumentNullException(nameof(ctor));
//            return this;
//        }

        public FluentBuilder DependsOn(RegistrationDependency dependency)
        {
            Registration.Dependencies.Add(dependency);
            return this;
        }

        public FluentBuilder UsingInstance(object instance)
        {
            Registration.Instance = instance;
            Registration.ScopedTo = new Provided();
            return this;
        }

        public FluentBuilder Scoped<T>() where T : ILifeSpan, new()
        {
            Registration.ScopedTo = new T();
            return this;
        }
    }

    public class FluentBuilder<TService>: FluentBuilder where TService : class
    {

        public FluentBuilder(Registration registration) : base(registration, typeof(TService))
        {
        }

        public FluentBuilder<TService> As<TContract>(string name = "default") where TContract : class
        {
            var contract = typeof(TContract);
            if (!contract.IsAssignableFrom(Registration.ImplementedType))
            {
                throw new ServiceDoesNotImplementContractException(contract, Registration.ImplementedType);
            }

            if (Registration.Types.Any(serviceKey=> serviceKey.ServiceName == name))
            {
                throw new DuplicateNamedContractException(contract, name);
            }
            
            Registration.Types.Add(new ServiceKey(typeof(TContract).GetTypeInfo(), name));
            return this;
        }

        public FluentBuilder<TService> DependsOn(RegistrationDependency dependency)
        {
            Registration.Dependencies.Add(dependency);
            return this;
        }

        public FluentBuilder<TService> DependsOn(Action<For> @for)
        {
            var dependency = new RegistrationDependency();
            var config = new For(dependency);
            @for(config);
            Registration.Dependencies.Add(dependency);
            return this;
        }

        public FluentBuilder<TService> UsingInstance(TService instance)
        {
            Registration.Instance = instance;
            Registration.ScopedTo = new Provided();
            return this;
        }

        public FluentBuilder<TService> Scoped<T>() where T : ILifeSpan, new()
        {
            Registration.ScopedTo = new T();
            return this;
        }
        
        public FluentBuilder<TService> Scoped<T>(Action<T> configScope) where T : ILifeSpan, new()
        {
            var scope = new T();
            configScope(scope);
            Registration.ScopedTo = scope;
            return this;
        }
    }
}