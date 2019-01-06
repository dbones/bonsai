namespace Bones.Tests.Lifestyles.Named
{
    using Machine.Specifications;
    using PowerAssert;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service1;
    using Named = Bones.Named;
    using Singleton = Bones.Singleton;
    using Transient = Bones.Transient;

    [Subject("NamedLifeScope")]
    public class When_disposing_the_scope 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            
            var parent = container.CreateScope("namedScope");
            _subject = parent.CreateScope("namedScope");

            var instanceWhichDoesNotGetDisposedOf = parent.Resolve<IService>();
            var instanceWhichGetsDisposedOf = _subject.Resolve<IService>();
            _monitor = _subject.Resolve<ClassMonitor>();
        };

        Because of = () => _subject.Dispose();
        
        It should_dispose_of_the_lowest_named_scope_services = 
            () => PAssert.IsTrue(() => _monitor.NumberOfDisposedInstances<IService>() == 1);
        
        static IScope _subject;
        static ClassMonitor _monitor;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ClassMonitor>().As<ClassMonitor>().Scoped<Singleton>();
                builder.Register<ServiceWithCtorAndDisposable>().As<IService>().Scoped<Named>(scope => scope.Name = "namedScope");
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}