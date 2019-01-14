namespace Bones.TestApp
{
    using System;
    using LifeStyles;
    using Registry;
    using Tests.TestModels.DataStore;
    using Tests.TestModels.Logger;
    using Tests.TestModels.Repository;
    using Tests.TestModels.Service2;

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.SetupModules(new BonesModule());

            using (var container = builder.Create())
            using (var scope = container.CreateScope())
            {
                var service = scope.Resolve<Service>();
                
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

    class BonesModule : IModule
    {
        public void Setup(ContainerBuilder builder)
        {
            builder.Register<Logger>().As<Logger>().Scoped<Transient>();
            builder.Register<Service>().As<Service>().Scoped<Transient>();
            builder.Register(typeof(Repository<>))
                .As(typeof(Repository<>)).Scoped<Transient>();
        }
    }
    
    public class Logger
    {
        
    }
    
    
    public class Repository<T>
    {
        public Logger Logger { get; }

        public Repository(Logger logger)
        {
            Logger = logger;
        }
        
    }
    
    
    public class Service
    {
        public Repository<User> User { get; }
        public Logger Logger { get; }

        public Service(Repository<User> user, Logger logger)
        {
            User = user;
            Logger = logger;
        }
    }


    public class User
    {
        
    }

}