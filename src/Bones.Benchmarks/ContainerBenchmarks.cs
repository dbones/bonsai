namespace Bones.Benchmarks
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Mathematics;
    using Castle.MicroKernel.Lifestyle;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Grace.DependencyInjection;
    using Registry;
    using ModuleRegistrationExtensions = Autofac.ModuleRegistrationExtensions;

    [InvocationCount(5000, 50)]
    [RankColumn(NumeralSystem.Stars)]
    [MemoryDiagnoser]
    public abstract class ContainerBenchmarks
    {
        private Bones.IContainer _bonesContainer;
        protected Bones.IScope _bonesScope;

        protected IWindsorContainer _windsorContainer;
        private IDisposable _windsorScope;

        private Autofac.IContainer _autofacContainer;
        protected Autofac.ILifetimeScope _autofacScope;

        private Grace.DependencyInjection.DependencyInjectionContainer _graceContainer;
        protected IExportLocatorScope _graceScope;

        
        [GlobalSetup]
        public void GlobalSetup()
        {
            var builder = new ContainerBuilder();
            builder.SetupModules(SetupBones());
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
            _windsorScope = _windsorContainer.BeginScope();
            _autofacScope = _autofacContainer.BeginLifetimeScope();
            _graceScope = _graceContainer.BeginLifetimeScope();
        }
        
        [IterationCleanup]
        public void Cleanup()
        {
            _bonesScope.Dispose();
            _windsorScope.Dispose();
            _autofacScope.Dispose();
            _graceScope.Dispose();
        }
        
        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _bonesContainer.Dispose();
            _windsorContainer.Dispose();
            _autofacContainer.Dispose();
            _graceContainer.Dispose();
        }
        

        protected abstract IModule SetupBones();
        protected abstract IWindsorInstaller SetupWindsor();
        protected abstract Autofac.Module SetupAutofac();
        protected abstract Grace.DependencyInjection.IConfigurationModule SetupGrace();
    }
}