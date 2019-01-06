namespace Bones.Tests.Resolving.Dependencies
{
    using Machine.Specifications;
    using PowerAssert;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service2;

    [Subject("Container")]
    public class When_resolving_a_service_with_parameter_injection_using_default_contract 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            
            _subject = container.CreateScope();
        };

        Because of = () => _service = _subject.Resolve<IService2>();

        It should_inject_a_dependency => () => PAssert.IsTrue(() => _service.Logger != null);
        It should_inject_an_instance_which_is_associated_with_the_registration = 
            () => PAssert.IsTrue(() => _service.Logger is LoggerPlain);
        
        static IScope _subject;
        static IService2 _service;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ServiceWithParameter>().As<IService2>().Scoped<Transient>();
                
                builder.Register<ClassMonitor>().As<ClassMonitor>().Scoped<Transient>();
                builder.Register<LoggerWithDisposable>().As<ILogger>("a").Scoped<Transient>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}