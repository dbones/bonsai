namespace Bonsai.Tests.Lifestyles.ProvidedInstance
{
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service1;
    using Singleton = LifeStyles.Singleton;
    using Transient = LifeStyles.Transient;

    [Subject("InstanceLifeScope")]
    public class When_disposing_the_scope 
    {
        private Establish context = () =>
        {
            _monitor = new ClassMonitor();
            var instance = new ServiceWithCtorAndDisposable(new LoggerPlain(), _monitor);
            
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts(instance));
            var container = builder.Create();
            
            var parent = container.CreateScope();
            _subject = parent.CreateScope();

            var instanceWhichDoesNotGetDisposedOf1 = parent.Resolve<IService>();
            var instanceWhichDoesNotGetDisposedOf2 = _subject.Resolve<IService>();
            var instanceWhichDoesNotGetDisposedOf3 = _subject.Resolve<IService>();
        };

        Because of = () => _subject.Dispose();
        
        It should_not_dispose_of_any_provided_instance = 
            () => PAssert.IsTrue(() => _monitor.NumberOfDisposedInstancesOf<ServiceWithCtorAndDisposable>() == 0);
        
        static IScope _subject;
        static ClassMonitor _monitor;
        
        class RegisterContracts : IModule
        {
            private readonly IService _service;

            public RegisterContracts(IService service)
            {
                _service = service;
            }
            
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ServiceWithCtorAndDisposable>().As<IService>().UsingInstance(_service);
            }
        }
    }
}