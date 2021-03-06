namespace Bonsai.Tests.Lifestyles.Named
{
    using System;
    using Exceptions;
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels.Logger;
    using TestModels.Service1;
    using Named = LifeStyles.Named;
    using Transient = LifeStyles.Transient;

    [Ignore("work in progress")]
    [Subject("NamedLifeScope")]
    public class When_resolving_a_service_outside_of_a_named_scope 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            _subject = container
                .CreateScope();
        };

        Because of = () => _exception = Catch.Exception(()=>  _subject.Resolve<IService>());
        
        It should_provide_scope_missing_exception = 
            () => PAssert.IsTrue(() => _exception is ScopeNotFoundException);
        
        
        static IScope _subject;
        static Exception _exception;
        
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