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
    public class When_disposing_the_container 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            _subject = builder.Create();
            
            var parent = _subject.CreateScope();
            var scope = parent.CreateScope();

            var instanceWhichGetsDisposedOf1 = parent.Resolve<IService>();
            var instanceWhichGetsDisposedOf2 = scope.Resolve<IService>();
            var instanceWhichGetsDisposedOf3 = scope.Resolve<IService>();
            _monitor = scope.Resolve<ClassMonitor>();
        };

        Because of = () => _subject.Dispose();
        
        It should_dispose_of_all_singleton_services = 
            () => PAssert.IsTrue(() => _monitor.NumberOfDisposedInstancesOf<ServiceWithCtorAndDisposable>() == 3);
        
        static IContainer _subject;
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