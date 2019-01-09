namespace Bones.TestApp
{
    using System;
    using Tests.TestModels.DataStore;
    using Tests.TestModels.Logger;
    using Tests.TestModels.Repository;
    using Tests.TestModels.Service2;

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.SetupModules(new SimpleModule());

            using (var container = builder.Create())
            using (var scope = container.CreateScope())
            {
                var service = scope.Resolve<IService2>();
                
            }
        }
    }

    public class SimpleModule : IModule
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