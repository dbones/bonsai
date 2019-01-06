namespace Bones.Tests.Lifestyles.ProvidedInstance
{
    using Machine.Specifications;
    using PowerAssert;
    using TestModels.Logger;
    using TestModels.Service1;

    [Subject("InstanceLifeScope")]
    public class When_resolving_a_service 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            
            _providedInstance = new ServiceWithCtor(new LoggerPlain());
            
            builder.SetupModules(new RegisterContracts(_providedInstance));
            var container = builder.Create();
            _subject = container.CreateScope();
        };

        Because of = () =>
        {
            _service = _subject.Resolve<IService>();
            _service2 = _subject.Resolve<IService>();
        };

        It should_provide_the_provided_instances = () => PAssert.IsTrue(() => _service == _providedInstance);
        It should_provide_the_same_instances = () => PAssert.IsTrue(() => _service == _service2);

        static IScope _subject;

        static IService _providedInstance;
        
        static IService _service;
        static IService _service2;
        
        class RegisterContracts : IModule
        {
            private readonly IService _service;

            public RegisterContracts(IService service)
            {
                _service = service;
            }
            
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ServiceWithCtor>().As<IService>().UsingInstance(_service);
            }
        }
    }
}