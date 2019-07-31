namespace Bonsai.Tests.Resolving.Funcs
{
    using LifeStyles;
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels.Logger;
    using TestModels.Service1;

    [Subject("Container")]
    public class When_resloving_instance_from_func 
    {
        private Establish context = () =>
        {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            var scope = container.CreateScope();
            _subject = scope.Resolve<FuncService>();
        };

        private Because of = () => _service = _subject.Logger();

        It should_indicate_instance_has_been_created = () => PAssert.IsTrue(() => _subject.Logger != null);
        It should_provide_a_lazy_instance = () => PAssert.IsTrue(() => _service is LoggerPlain);

        static ILogger _service;
        static FuncService _subject;

        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<FuncService>().As<FuncService>().Scoped<Transient>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}