namespace Bonsai.Tests.Resolving.Dependencies
{
    using LifeStyles;
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service1;

    [Subject("Container")]
    public class When_resolving_a_named_constructor_dependency_using_parameter_type 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            
            _subject = container.CreateScope();
        };

        Because of = () => _service = _subject.Resolve<IService>();

        It should_inject_a_dependency = () => PAssert.IsTrue(() => _service.Logger != null);
        It should_inject_an_instance_which_is_associated_with_the_registration = 
            () => PAssert.IsTrue(() => _service.Logger is LoggerPlain);
        
        static IScope _subject;
        static IService _service;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ServiceWithCtor>().As<IService>().Scoped<Transient>()
                    .DependsOn(x => x.Constructor().Parameter("logger").Named("b"));
                
                builder.Register<ClassMonitor>().As<ClassMonitor>().Scoped<Transient>();
                builder.Register<LoggerWithDisposable>().As<ILogger>("a").Scoped<Transient>();
                builder.Register<LoggerPlain>().As<ILogger>("b").Scoped<Transient>();
            }
        }
    }
}