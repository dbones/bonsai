namespace Bonsai.Benchmarks
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Mathematics;
    using Castle.MicroKernel.Lifestyle;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Grace.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Registry;
    using ModuleRegistrationExtensions = Autofac.ModuleRegistrationExtensions;

    [InvocationCount(5000, 50)]
    [RankColumn(NumeralSystem.Stars)]
    [MemoryDiagnoser]
    public abstract class ContainerBenchmarks
    {
        private Bonsai.IContainer _bonesContainer;
        protected Bonsai.IScope _bonesScope;

        protected IWindsorContainer _windsorContainer;
        private IDisposable _windsorScope;

        private ServiceProvider _msContainer;
        protected IServiceScope _msScope;

        private Autofac.IContainer _autofacContainer;
        protected Autofac.ILifetimeScope _autofacScope;

        private Grace.DependencyInjection.DependencyInjectionContainer _graceContainer;
        protected IExportLocatorScope _graceScope;

        
        [GlobalSetup]
        public virtual void GlobalSetup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.Setup(SetupMs());
            _msContainer = serviceCollection.BuildServiceProvider();

            var builder = new ContainerBuilder();
            builder.SetupModules(SetupBonsai());
            _bonesContainer= builder.Create();

            _windsorContainer = new WindsorContainer();
            _windsorContainer.Install(SetupWindsor());

            var autofacBuilder = new Autofac.ContainerBuilder();
            ModuleRegistrationExtensions.RegisterModule(autofacBuilder, SetupAutofac());
            _autofacContainer = autofacBuilder.Build();
            
            _graceContainer = new Grace.DependencyInjection.DependencyInjectionContainer();
            _graceContainer.Configure(SetupGrace());
        }

        [IterationSetup]
        public void Setup()
        {
            _bonesScope = _bonesContainer.CreateScope();
            _msScope = _msContainer.CreateScope();
            _windsorScope = _windsorContainer.BeginScope();
            _autofacScope = _autofacContainer.BeginLifetimeScope();
            _graceScope = _graceContainer.BeginLifetimeScope();
        }
        
        [IterationCleanup]
        public void Cleanup()
        {
            _bonesScope.Dispose();
            _msScope.Dispose();
            _windsorScope.Dispose();
            _autofacScope.Dispose();
            _graceScope.Dispose();
        }
        
        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _bonesContainer.Dispose();
            _msContainer.Dispose();
            _windsorContainer.Dispose();
            _autofacContainer.Dispose();
            _graceContainer.Dispose();
        }
        

        protected abstract IModule SetupBonsai();
        protected abstract IWindsorInstaller SetupWindsor();
        protected abstract Autofac.Module SetupAutofac();
        protected abstract Grace.DependencyInjection.IConfigurationModule SetupGrace();
        protected abstract IServiceCollectionModule SetupMs();
    }


    public interface IServiceCollectionModule
    {
        void Setup(IServiceCollection serviceCollection);
    }

    public static class ServiceCollectionExtensions
    {
        public static void Setup(this IServiceCollection collection, params IServiceCollectionModule[] modules)
        {
            foreach (var serviceCollectionModule in modules)
            {
                serviceCollectionModule.Setup(collection);
            }
        }
    }
}