namespace Bones.Tests.Resolving.Collections
{
    using System.Collections.Generic;
    using System.Linq;
    using Machine.Specifications;
    using PowerAssert;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service1;

    [Ignore("work in progress")]
    [Subject("Container")]
    public class When_resolving_all_services_of_a_type 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            
            _subject = container.CreateScope();
        };

        Because of = () => _services = _subject.ResolveAll<IService>();

        
        It should_provide_all_instances = () => PAssert.IsTrue(() => _services.Count() == 2);
        
        It should_provide_an_instance_of_the_default_service = 
            () => PAssert.IsTrue(() => _services.Any(x=> x is ServiceWithCtor));
        
        It should_provide_an_instance_of_the_named_service = 
            () => PAssert.IsTrue(() => _services.Any(x=> x is ServiceWithCtorAndDisposable));
        
        static IScope _subject;
        static IEnumerable<IService> _services;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ClassMonitor>().As<ClassMonitor>().Scoped<Singleton>();
                builder.Register<ServiceWithCtorAndDisposable>().As<IService>("named").Scoped<Singleton>();
                builder.Register<ServiceWithCtor>().As<IService>().Scoped<Transient>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}