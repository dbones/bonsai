namespace Bones.Tests.Lifestyles.Scoped
{
    using Machine.Specifications;
    using PowerAssert;
    using TestModels.Logger;
    using TestModels.Service1;
    using Named = Bones.Named;
    using Transient = Bones.Transient;

    [Subject("ScopedLifeScope")]
    public class When_resolving_a_service 
    {
        private Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();

            var parent = container.CreateScope(); 
            _service = parent.Resolve<IService>();
            
            _subject = parent.CreateScope();
        };

        Because of = () => _service2 = _subject.Resolve<IService>();

        It should_create_an_instance = () => PAssert.IsTrue(() => _service != null);
        It should_provide_a_new_instance_at_different_scopes = () => PAssert.IsTrue(() => _service != _service2);

        static IScope _subject;
        static IService _service;
        static IService _service2;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ServiceWithCtor>().As<IService>().Scoped<PerScope>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}