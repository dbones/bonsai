using Bonsai.LifeStyles;
using Bonsai.Registry;
using Bonsai.Tests.TestModels;
using Bonsai.Tests.TestModels.Logger;
using Machine.Specifications;
using PowerAssert;

namespace Bonsai.Tests.Resolving
{
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
            () => PAssert.IsTrue(() => _service is LoggerWithDisposable);
        
        static IScope _subject;
        static ILogger _service;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<LoggerWithDisposable>().As<ILogger>().Scoped<Transient>();
                builder.Register<ClassMonitor>().As<ClassMonitor>().UsingInstance(new ClassMonitor());
            }
        }
    }
}