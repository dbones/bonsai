namespace Bones.Tests.Resolving.NamedInstances
{
    using Machine.Specifications;
    using PowerAssert;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service1;

    [Subject("Container")]
    public class When_resolving_the_default_service 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            _subject = container.CreateScope();
        };

        Because of = () => _service = _subject.Resolve<IService>();

        It should_provide_an_instance => () => PAssert.IsTrue(() => _service != null);
        It should_provide_the_default_instance = 
            () => PAssert.IsTrue(() => _service is ServiceWithCtorAndDisposable);
        
        static IScope _subject;
        static IService _service;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ClassMonitor>().As<ClassMonitor>().Scoped<Singleton>();
                builder.Register<ServiceWithCtorAndDisposable>().As<IService>().Scoped<Singleton>();
                builder.Register<ServiceWithCtor>().As<IService>("simple").Scoped<Transient>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}