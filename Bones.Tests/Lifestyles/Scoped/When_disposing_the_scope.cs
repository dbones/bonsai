namespace Bones.Tests.Lifestyles.Scoped
{
    using Machine.Specifications;
    using PowerAssert;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service1;
    using Named = Bones.Named;
    using Singleton = Bones.Singleton;
    using Transient = Bones.Transient;

    [Subject("ScopedLifeScope")]
    public class When_disposing_the_scope 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            
            var parent = container.CreateScope();
            _subject = parent.CreateScope();

            var instanceWhichDoesNotGetDisposedOf = parent.Resolve<IService>();
            var instanceWhichGetsDisposedOf = _subject.Resolve<IService>();
            _monitor = _subject.Resolve<ClassMonitor>();
        };

        Because of = () => _subject.Dispose();
        
        It should_dispose_of_the_lowest_scoped_services = 
            () => PAssert.IsTrue(() => _monitor.NumberOfDisposedInstancesOf<ServiceWithCtorAndDisposable>() == 1);
        
        static IScope _subject;
        static ClassMonitor _monitor;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ClassMonitor>().As<ClassMonitor>().Scoped<Singleton>();
                builder.Register<ServiceWithCtorAndDisposable>().As<IService>().Scoped<PerScope>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}