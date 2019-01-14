namespace Bones.Tests.Resolving
{
    using LifeStyles;
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels.Logger;

    [Subject("Container")]
    public class When_resolving_a_simple_service 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            
            _subject = container.CreateScope();
        };

        Because of = () => _service = _subject.Resolve<ILogger>();

        It should_provide_an_instance => () => PAssert.IsTrue(() => _service != null);
        It should_provide_an_instance_which_is_associated_with_the_registration = 
            () => PAssert.IsTrue(() => _service is LoggerPlain);
        
        static IScope _subject;
        static ILogger _service;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
    
    [Subject("Container")]
    public class When_resolving_a_complex_service 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            
            _subject = container.CreateScope();
        };

        Because of = () => _service = _subject.Resolve<ILogger>();

        It should_provide_an_instance = () => PAssert.IsTrue(() => _service != null);
        It should_provide_an_instance_which_is_associated_with_the_registration = 
            () => PAssert.IsTrue(() => _service is LoggerPlain);
        
        static IScope _subject;
        static ILogger _service;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
    
    
}