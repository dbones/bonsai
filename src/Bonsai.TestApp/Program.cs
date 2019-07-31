namespace Bonsai.TestApp
{
    using System;
    using Contracts;
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
            builder.SetupModules(new BonsaiModule());

            using (var container = builder.Create())
            {
                SomeTestCode(container);
            }
            
            Console.WriteLine("done");
        }

        private static void Ctor(IContainer container)
        {
            var repoContract = new Contract(1);
            var logContract = new Contract(2);
            
            using (var scope = (IAdvancedScope)container.CreateScope())
            {
                
                var d = new Service((Repository<User>)scope.Resolve(repoContract), (Logger)scope.Resolve(logContract));
                
            }
        }

        private static void SomeTestCode(IContainer container)
        {
            using (var scope = container.CreateScope())
            {
                var service = scope.Resolve<Service>();
                service.Transfer(13.34m, "A", "B");
            }

            for (int i = 0; i < 100; i++)
            {
                using (var scope = container.CreateScope())
                {
                    var service = scope.Resolve<Service>();
                    service.Transfer(13.34m, "A", "B");


                    for (int j = 0; j < 100; j++)
                    {
                        using (var scope2 = scope.CreateScope())
                        {
                            var service2 = scope2.Resolve<Service>();
                        }
                    }
                }
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

    class BonsaiModule : IModule
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
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
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

        public void Transfer(decimal amount, string source, string destination)
        {
            Logger.Write($"transfer {amount}. {source} => {destination}");
        }
    }


    public class User
    {
        
    }

}