namespace Bones.Tests.Resolving.Lazyness
{
    using System;
    using NUnit.Specifications;
    using PowerAssert;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service1;

    [Subject("Container")]
    public class When_resloving_instance_from_lazy : ContextSpecification
    {
        private Establish context = () =>
        {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            var scope = container.CreateScope();
            _subject = scope.Resolve<Lazy<IService>>();
        };

        private Because of = () => _service = _subject.Value;

        It should_indicate_instance_has_been_created => () => PAssert.IsTrue(() => _subject.IsValueCreated);
        It should_provide_a_lazy_instance => () => PAssert.IsTrue(() => _service is ServiceWithCtor);

        static IService _service;
        static Lazy<IService> _subject;

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