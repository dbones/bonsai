namespace Bonsai.Tests.Resolving.Lazyness
{
    using System;
    using LifeStyles;
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels;
    using TestModels.Logger;
    using TestModels.Service1;

    [Subject("Container")]
    public class When_resloving_instance_from_lazy 
    {
        private Establish context = () =>
        {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            var scope = container.CreateScope();
            _subject = scope.Resolve<LazyService>();
        };

        private Because of = () => _service = _subject.Logger.Value;

        It should_indicate_instance_has_been_created = () => PAssert.IsTrue(() => _subject.Logger.IsValueCreated);
        It should_provide_a_lazy_instance = () => PAssert.IsTrue(() => _service is LoggerPlain);

        static ILogger _service;
        static LazyService _subject;

        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<LazyService>().As<LazyService>().Scoped<Transient>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}