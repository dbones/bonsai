namespace Bones.Tests.Lifestyles.Singleton
{
    using System;
    using Machine.Specifications;
    using PowerAssert;
    using TestModels.Logger;
    using TestModels.Service1;
    using Singleton = Bones.Singleton;
    using Transient = Bones.Transient;

    [Subject("SingletonLifeScope")]
    public class When_resolving_a_service 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            _subject = container.CreateScope();
        };

        Because of = () =>
        {
            _service = _subject.Resolve<IService>();
            _service2 = _subject.Resolve<IService>();
        };

        It should_create_an_instance = () => PAssert.IsTrue(() => _service != null);
        It should_provide_the_same_instances = () => PAssert.IsTrue(() => _service == _service2);

        private Cleanup after = () => _subject.Dispose();

        static IScope _subject;
        
        static IService _service;
        static IService _service2;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ServiceWithCtor>().As<IService>().Scoped<Singleton>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}