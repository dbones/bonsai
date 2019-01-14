namespace Bonsai.Benchmarks
{
    using Autofac;
    using BenchmarkDotNet.Attributes;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using LifeStyles;
    using Registry;
    using ContainerBuilder = Bonsai.ContainerBuilder;
    using Module = Autofac.Module;


    public class SingletonBench : ContainerBenchmarks
    {
        [Benchmark(Baseline = true)]
        public Service Bonsai() => _bonesScope.Resolve<Service>();

        [Benchmark]
        public Service Windsor() => _windsorContainer.Resolve<Service>();
        
        [Benchmark]
        public Service Autofac() => _autofacScope.Resolve<Service>();
        
        [Benchmark]
        public Service Grace() => _graceScope.Locate<Service>();

        protected override IModule SetupBonsai() => new BonsaiModule();

        protected override IWindsorInstaller SetupWindsor() => new WindsorInstaller();
        

        protected override Module SetupAutofac() => new AutofacModule();
        protected override Grace.DependencyInjection.IConfigurationModule SetupGrace() => new GraveModule();

        
        class BonsaiModule : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<Logger>().As<Logger>().Scoped<Singleton>();
                builder.Register<Service>().As<Service>().Scoped<Singleton>();
                builder.Register(typeof(Repository<>))
                    .As(typeof(Repository<>)).Scoped<Singleton>();
            }
        }
        
        class WindsorInstaller : IWindsorInstaller
        {
            public void Install(IWindsorContainer container, IConfigurationStore store)
            {
                container.Register(
                    Component.For<Logger>().ImplementedBy<Logger>().LifestyleSingleton(),
                    Component.For<Service>().ImplementedBy<Service>().LifestyleSingleton(),
                    Component.For(typeof(Repository<>))
                        .ImplementedBy(typeof(Repository<>)).LifestyleSingleton()
                );
            }
        }

        class AutofacModule : Autofac.Module
        {
            protected override void Load(Autofac.ContainerBuilder builder)
            {
                builder.RegisterType<Logger>().AsSelf().SingleInstance();
                builder.RegisterType<Service>().AsSelf().SingleInstance();
                builder.RegisterGeneric(typeof(Repository<>)).AsSelf().SingleInstance();
            }
        }
        
        class GraveModule : Grace.DependencyInjection.IConfigurationModule
        {
            public void Configure(Grace.DependencyInjection.IExportRegistrationBlock builder)
            {
                builder.Export<Logger>().As<Logger>().Lifestyle.Singleton();
                builder.Export<Service>().As<Service>().Lifestyle.Singleton();
                builder.Export(typeof(Repository<>)).As(typeof(Repository<>)).Lifestyle.Singleton();
            }
        }
    }
}