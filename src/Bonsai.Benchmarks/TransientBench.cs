namespace Bonsai.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Autofac;
    using BenchmarkDotNet.Attributes;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Grace.DependencyInjection;
    using LifeStyles;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Registry;
    using ContainerBuilder = Bonsai.ContainerBuilder;

    public class TransientBench : ContainerBenchmarks
    {
        [Benchmark(Baseline = true)]
        public Service Bonsai() => _bonesScope.Resolve<Service>();

        [Benchmark]
        public Service Microsoft() => _msScope.ServiceProvider.GetService<Service>();


        [Benchmark]
        public Service Windsor() => _windsorContainer.Resolve<Service>();
        
        [Benchmark]
        public Service Autofac() => _autofacScope.Resolve<Service>();
        
        [Benchmark]
        public Service Grace() => _graceScope.Locate<Service>();

        [Benchmark]
        public Service Direct() => new Service(new Repository<User>(new Logger()), new Logger());

        [Benchmark]
        public Service Lambda() => getService();

        [Benchmark]
        public Service SimpleIoc() => simpleIoc.Resolve<Service>();


        static Func<Logger> getLogger = () => new Logger();
        static Func<Repository<User>> getRepo = () => new Repository<User>(getLogger());
        static Func<Service> getService = () => new Service(getRepo(), getLogger());
        SimpleIoc simpleIoc = new SimpleIoc();

        public override void GlobalSetup()
        {
            base.GlobalSetup();
            simpleIoc.Add(()=> new Logger());
            simpleIoc.Add(()=> new Repository<User>(simpleIoc.Resolve<Logger>()));
            //simpleIoc.Add(()=> new Service(simpleIoc.Resolve<Repository<User>>(), simpleIoc.Resolve<Logger>()));
            simpleIoc.Add(() => new Service(new Repository<User>(new Logger()), new Logger()));

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

}