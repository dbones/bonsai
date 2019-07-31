namespace Bonsai
{
    using System;
    using System.Linq.Expressions;
    using Contracts;
    using Registry;

    public static class ContainerBuilderExtensions
    {
        public static FluentBuilder<T> Register<T>(this ContainerBuilder builder) where T : class
        {
            var registration = new Registration();
            builder.RegisterContract(registration);
            return new FluentBuilder<T>(registration);
        }
        
        public static FluentBuilder<T> Register<T>(this ContainerBuilder builder, Expression<Func<IScope, T>> ctor) where T : class
        {
            var compiled = ctor.Compile();
            
            var registration = new Registration();
            builder.RegisterContract(registration);
            registration.CreateInstance = (scope, contract, parentContract) => compiled(scope); 
            return new FluentBuilder<T>(registration);
        }
                
        public static FluentBuilder<T> Register<T>(this ContainerBuilder builder, Expression<Func<IAdvancedScope, Contract, Contract, T>> ctor) where T : class
        {
            var registration = new Registration();
            
            builder.RegisterContract(registration);
            var compiled = ctor.Compile();
            registration.CreateInstance =
                (scope, contract, parentContract) => compiled(scope, contract, parentContract);
            return new FluentBuilder<T>(registration);
        }
        
        
        public static FluentBuilder Register(this ContainerBuilder builder, Type serviceType)
        {
            var registration = new Registration();
            builder.RegisterContract(registration);
            return new FluentBuilder(registration, serviceType);
        }
        
        public static FluentBuilder Register(this ContainerBuilder builder, Type serviceType, Expression<CreateInstance> ctor)
        {
            //var compiled = ctor.Compile();
            
            var registration = new Registration();
            builder.RegisterContract(registration);
            registration.CreateInstance = ctor.Compile(); 
            return new FluentBuilder(registration, serviceType);
        }

        //public static FluentBuilder Register(this ContainerBuilder builder, Type serviceType, Expression<Func<IScope, Contract, object>> ctor)
        //{
        //    var compiled = ctor.Compile();

        //    var registration = new Registration();
        //    builder.RegisterContract(registration);
        //    registration.CreateInstance = (scope, contract, parentContract) => compiled(scope, contract);
        //    return new FluentBuilder(registration, serviceType);
        //}
    }
}