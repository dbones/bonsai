namespace Bonsai.Internal
{
    using System;
    using System.Collections.Generic;
    using LifeStyles;
    using Registry;
    using Types;

    public class InternalModule : IModule
    {
        public void Setup(ContainerBuilder builder)
        {
            builder.Register(scope => (Scope)scope).As<IScope>().As<IAdvancedScope>()
                .Scoped<PerScope>();

            builder.Register(typeof(EnumerableService<>)).As(typeof(IEnumerable<>))
                .Scoped<Transient>();

            builder.Register(typeof(LazyService<>)).As(typeof(Lazy<>))
                //.DependsOn(x => x.Constructor().ParameterWithType<Contract>(), (_, c, contract) => contract.ServiceKeys.First().Service.GetGenericArguments()[0])
                .Scoped<Transient>();
            
        }
    }
    
}