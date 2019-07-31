namespace Bonsai.Benchmarks
{
    using Autofac;
    using BenchmarkDotNet.Attributes;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Grace.DependencyInjection;
    using LifeStyles;
    using Models;
    using Registry;
    using ContainerBuilder = Bonsai.ContainerBuilder;

    public class TransientBench : ContainerBenchmarks
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
        protected override IConfigurationModule SetupGrace() => new GraveModule();
        

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
}