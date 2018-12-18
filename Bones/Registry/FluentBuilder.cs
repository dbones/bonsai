namespace Bones
{
    using System;
    using System.Reflection;
    
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

            Registration.Types.TryAdd(name, typeof(TContract).GetTypeInfo());
            return this;
        }
        
        public FluentBuilder As(Type serviceType, string name = "default") 
        {
            var contract = serviceType.GetTypeInfo();
            if (!contract.IsAssignableFrom(Registration.ImplementedType))
            {
                //TODO: Exceptions
            }

            Registration.Types.TryAdd(name, contract);
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
            return this;
        }

        public FluentBuilder Scoped<T>() where T : ILifeSpan, new()
        {
            throw new NotImplementedException();
        }
    }

    public class FluentBuilder<TService>: FluentBuilder where TService : class
    {

        public FluentBuilder(Registration registration) : base(registration, typeof(TService))
        {
        }

        public FluentBuilder<TService> As<TContract>(string name = "default") where TContract : class
        {
            var contract = typeof(TContract).GetTypeInfo();
            if (!contract.IsAssignableFrom(Registration.ImplementedType))
            {
                //TODO: Exceptions
            }

            Registration.Types.TryAdd(name, typeof(TContract).GetTypeInfo());
            return this;
        }


//        public FluentBuilder<TService> ConstructUsing(Func<ResolvingContext, TService> ctor)
//        {
//            Registration.Constructor = ctor ?? throw new ArgumentNullException(nameof(ctor));
//            return this;
//        }

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
            Registration.Instance = scope;
            return this;
        }
    }
}