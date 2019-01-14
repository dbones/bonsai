namespace Bones.Tests.Resolving.Generics
{
    using LifeStyles;
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels;
    using TestModels.DataStore;
    using TestModels.Logger;
    using TestModels.Repository;
    using TestModels.Service2;

    [Subject("Container")]
    public class When_resolving_a_service_with_generic_dependencies 
    {
        Establish context = () =>
        {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            _subject = container.CreateScope();
        };

        Because of = () => _service = _subject.Resolve<IService2>() as ServiceWith2ParameterCtor;

        It should_provided_an_instance_of_the_service =
            () => PAssert.IsTrue(() => _service != null);
        
        It should_have_a_generic_dependency_set = 
            () => PAssert.IsTrue(() => _service.Repository != null);
        
        It should_have_a_nested_generic_dependency_set = 
            () => PAssert.IsTrue(() => 
                ((InMemoryRespositoryWith1ParamGenericCtor<User>)_service.Repository).DataStore != null);
        

        static IScope _subject;
        static ServiceWith2ParameterCtor _service;

        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ServiceWith2ParameterCtor>()
                    .As<IService2>()
                    .Scoped<Transient>();
                
                builder.Register<LoggerPlain>()
                    .As<ILogger>()
                    .Scoped<Transient>();
                
                builder.Register(typeof(InMemoryRespositoryWith1ParamGenericCtor<>))
                    .As(typeof(IMemoryRepository<>))
                    .As(typeof(IRepository<>))
                    .Scoped<Transient>();
                
                builder.Register(typeof(DataStorePlain<>))
                    .As(typeof(IDataStore<>))
                    .Scoped<Transient>();
            }
        }
    }
}