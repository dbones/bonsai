namespace Bonsai.Tests.Resolving.Funcs
{
    using LifeStyles;
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels.Logger;
    using TestModels.Service1;

    [Subject("Container")]
    public class When_resolving_services_as_a_Func_type 
    {
        Establish context = () =>
        {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            _subject = container.CreateScope();
        };

        Because of = () => _service = _subject.Resolve<FuncService>();

        It should_have_created_a_lazy_instance = () => PAssert.IsTrue(() => _service.Logger != null);

        static IScope _subject;
        static FuncService _service;

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