namespace Bones.Benchmarks
{
    using Autofac;
    using BenchmarkDotNet.Attributes;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using ContainerBuilder = Bones.ContainerBuilder;

    public class SingletonBench : ContainerBenchmarks
    {
        [Benchmark(Baseline = true)]
        public Service Bones() => _bonesScope.Resolve<Service>();

        [Benchmark]
        public Service Windsor() => _windsorContainer.Resolve<Service>();
        
        [Benchmark]
        public Service Autofac() => _autofacScope.Resolve<Service>();

        protected override IModule SetupBones() => new BonesModule();

        protected override IWindsorInstaller SetupWindsor() => new WindsorInstaller();
        

        protected override Module SetupAutofac() => new AutofacModule();
        
        class BonesModule : IModule
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
    }
}