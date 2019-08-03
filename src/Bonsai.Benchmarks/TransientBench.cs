namespace Bonsai.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using BenchmarkDotNet.Attributes;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Contracts;
    using Grace.DependencyInjection;
    using ImTools;
    using Internal;
    using LifeStyles;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Registry;
    using ContainerBuilder = Bonsai.ContainerBuilder;

    public class TransientBench : ContainerBenchmarks
    {
        [Benchmark(Baseline = true)]
        public Service Bonsai() => _bonesScope.Resolve<Service>();

        //[Benchmark]
        public Service Microsoft() => _msScope.ServiceProvider.GetService<Service>();

        //[Benchmark]
        public Service Windsor() => _windsorContainer.Resolve<Service>();
        
        //[Benchmark]
        public Service Autofac() => _autofacScope.Resolve<Service>();
        
        //[Benchmark]
        public Service Grace() => _graceScope.Locate<Service>();

        [Benchmark]
        public Service Direct() => new Service(new Repository<User>(new Logger()), new Logger());

        [Benchmark]
        public Service Lambda() => getService();

        [Benchmark]
        public Service SimpleIoc1() => simpleIoc1.Resolve<Service>();

        [Benchmark]
        public Service SimpleIoc2() => simpleIoc2.Resolve<Service>();

        [Benchmark]
        public Service SimpleIoc3() => simpleIoc3.Resolve<Service>();


        static Func<Logger> getLogger = () => new Logger();
        static Func<Repository<User>> getRepo = () => new Repository<User>(getLogger());
        static Func<Service> getService = () => new Service(getRepo(), getLogger());


        SimpleIoc simpleIoc1 = new SimpleIoc();
        SimpleIoc2 simpleIoc2 = new SimpleIoc2();
        SimpleIoc3 simpleIoc3 = new SimpleIoc3();

        public override void GlobalSetup()
        {
            base.GlobalSetup();
            //simpleIoc.Add(()=> new Logger());
            //simpleIoc.Add(()=> new Repository<User>(simpleIoc.Resolve<Logger>()));
            //simpleIoc.Add(()=> new Service(simpleIoc.Resolve<Repository<User>>(), simpleIoc.Resolve<Logger>()));
            //simpleIoc.Add(() => new Service(new Repository<User>(new Logger()), new Logger()));


            simpleIoc1.Add(getService);
            simpleIoc2.Add(getService);
            simpleIoc3.Add(getService);
        }

        protected override IModule SetupBonsai() => new BonsaiModule();
        protected override IWindsorInstaller SetupWindsor() => new WindsorInstaller();
        protected override Module SetupAutofac() => new AutofacModule();
        protected override IConfigurationModule SetupGrace() => new GraveModule();
        protected override IServiceCollectionModule SetupMs() => new MsModule();


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

        class MsModule : IServiceCollectionModule
        {
            public void Setup(IServiceCollection serviceCollection)
            {
                serviceCollection.AddTransient<Logger, Logger>();
                serviceCollection.AddTransient<Service, Service>();
                serviceCollection.AddTransient(typeof(Repository<>), typeof(Repository<>));
            }
        }

        class WindsorInstaller : IWindsorInstaller
        {
            public void Install(IWindsorContainer container, IConfigurationStore store)
            {
                container.Register(
                    Component.For<Logger>().ImplementedBy<Logger>().LifestyleTransient(),
                    Component.For<Service>().ImplementedBy<Service>().LifestyleTransient(),
                    Component.For(typeof(Repository<>))
                        .ImplementedBy(typeof(Repository<>)).LifestyleTransient()
                );
            }
        }

        class AutofacModule : Autofac.Module
        {
            protected override void Load(Autofac.ContainerBuilder builder)
            {
                builder.RegisterType<Logger>().AsSelf().InstancePerDependency();
                builder.RegisterType<Service>().AsSelf().InstancePerDependency();
                builder.RegisterGeneric(typeof(Repository<>)).AsSelf().InstancePerDependency();
            }
        }
        
        class GraveModule : Grace.DependencyInjection.IConfigurationModule
        {
            public void Configure(IExportRegistrationBlock builder)
            {
                builder.Export<Logger>().As<Logger>();
                builder.Export<Service>().As<Service>();
                builder.Export(typeof(Repository<>)).As(typeof(Repository<>));
            }
        }
    }


    public class SimpleIoc
    {
        private Dictionary<Type, CreateInstance> lookup = new Dictionary<Type, CreateInstance>();
        public void Add<T>(Func<T> @delegate) => lookup.Add(typeof(T), () => @delegate());
        public T Resolve<T>() => (T) Resolve(typeof(T));
        public object Resolve(Type t) => lookup[t]();
        delegate object CreateInstance();
    }

    public class SimpleIoc2
    {
        private ImHashMap<Type, CreateInstance> lookup2 = ImHashMap<Type, CreateInstance>.Empty;
        public void Add<T>(Func<T> @delegate) => lookup2 = lookup2.AddOrUpdate(typeof(T), ()=> @delegate());

        public T Resolve<T>() => (T)Resolve(typeof(T));
        public object Resolve(Type t)
        {
            lookup2.TryFind(t, out var entry);
            return entry();
        }

        delegate object CreateInstance();
    }


    public class SimpleIoc3
    {
        private ImHashMap<ServiceKey, CreateInstance> lookup2 = ImHashMap<ServiceKey, CreateInstance>.Empty;
        public void Add<T>(Func<T> @delegate) => lookup2 = lookup2.AddOrUpdate(new ServiceKey(typeof(T)), () => @delegate());

        public T Resolve<T>() => (T)Resolve(typeof(T));
        public object Resolve(Type t)
        {
            lookup2.TryFind(new ServiceKey(t), out var entry);
            return entry();
        }

        delegate object CreateInstance();
    }


    delegate object CreateInstance(IAdvancedScope s, Contract c, Contract c2);

    class Builder
    {
        
        public object Test1(IAdvancedScope scope, Contract contract, List<object> paramsCache, List<Contract> contractCache, List<CreateInstance> delegateCache)
        {
            return new Service((Repository<User>)scope.Resolve(contractCache[487] ,contract), (Logger)paramsCache[234]);
        }

        public object Test2(IAdvancedScope scope, Contract contract, List<object> paramsCache, List<Contract> contractCache, List<CreateInstance> delegateCache)
        {
            return new Service((Repository<User>)paramsCache[4], (Logger)scope.Resolve(contractCache[7], contract));
        }

        public object Test3(IAdvancedScope scope, Contract contract, List<object> paramsCache, List<Contract> contractCache, List<CreateInstance> delegateCache)
        {
            return new Service((Repository<User>)paramsCache[4], (Logger)scope.Resolve(contractCache[234], contract));
        }

        public object Test4(IAdvancedScope scope, Contract contract, List<object> paramsCache, List<Contract> contractCache, List<CreateInstance> delegateCache)
        {
            return new Service((Repository<User>)paramsCache[4], (Logger)delegateCache[3455](scope, null, contract));
        }

        public object Test5(IAdvancedScope scope, Contract contract, List<object> paramsCache, List<Contract> contractCache, List<CreateInstance> delegateCache)
        {
            Console.Write(scope);
            Console.Write(contract);
            Console.Write(paramsCache);
            Console.Write(contractCache);
            Console.Write(delegateCache);

            return null;
        }









    }

}