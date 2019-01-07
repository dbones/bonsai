namespace Bones.Tests.Lifestyles.Singleton
{
    using Machine.Specifications;
    using PowerAssert;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service1;
    using Singleton = Bones.Singleton;
    using Transient = Bones.Transient;

    [Subject("SingletonLifeScope")]
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
            var instanceWhichGetsDisposedOf2 = _subject.Resolve<IService>();
            _monitor = _subject.Resolve<ClassMonitor>();
        };

        Because of = () => _subject.Dispose();
        
        It should_not_dispose_of_any_singleton_service_at_that_scope = 
            () => PAssert.IsTrue(() => _monitor.NumberOfDisposedInstancesOf<ServiceWithCtorAndDisposable>() == 0);
        
        static IScope _subject;
        static ClassMonitor _monitor;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ClassMonitor>().As<ClassMonitor>().Scoped<Singleton>();
                builder.Register<ServiceWithCtorAndDisposable>().As<IService>().Scoped<Singleton>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}