namespace Bones.Tests.Lifestyles.Named
{
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels.Logger;
    using TestModels.Service1;
    using Named = LifeStyles.Named;
    using Transient = LifeStyles.Transient;

    [Subject("NamedLifeScope")]
    public class When_resolving_a_service 
    {
        private Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();

            var namedScope = container
                .CreateScope("namedScope"); //services will be resolved from this scope 
            
            _service = namedScope.Resolve<IService>();
            
            _subject = namedScope
                .CreateScope();
        };

        Because of = () => _service2 = _subject.Resolve<IService>();

        It should_create_an_instance = () => PAssert.IsTrue(() => _service != null);
        It should_provide_the_same_instances = () => PAssert.IsTrue(() => _service == _service2);

        static IScope _subject;
        static IService _service;
        static IService _service2;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ServiceWithCtor>().As<IService>().Scoped<Named>(scope => scope.Name = "namedScope");
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}