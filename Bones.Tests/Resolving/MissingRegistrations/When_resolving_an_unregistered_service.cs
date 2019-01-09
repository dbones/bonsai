namespace Bones.Tests.Resolving.MissingRegistrations
{
    using System;
    using Exceptions;
    using Machine.Specifications;
    using PowerAssert;
    using TestModels.Logger;
    using TestModels.Service2;

    [Subject("Container")]
    public class When_resolving_an_unregistered_service 
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            _subject = container.CreateScope();
        };

        Because of = () => _exception = Catch.Exception(()=> _subject.Resolve<IService2>());

        It should_throw_an_exception = () => PAssert.IsTrue(() => _exception is ContractNotSupportedException);
        
        static IScope _subject;
        static Exception _exception;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
            }
        }
    }
}