namespace Bones.Tests.Resolving.Lazyness
{
    using System;
    using Machine.Specifications;
    using PowerAssert;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service1;

    [Ignore("wip")]
    [Subject("Container")]
    public class When_resolving_services_as_a_Lazy_type 
    {
        Establish context = () =>
        {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            _subject = container.CreateScope();
        };

        Because of = () => _service = _subject.Resolve<Lazy<IService>>();

        It should_have_created_a_lazy_instance = () => PAssert.IsTrue(() => !_service.IsValueCreated);

        static IScope _subject;
        static Lazy<IService> _service;

        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ServiceWithCtor>().As<IService>().Scoped<Transient>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}