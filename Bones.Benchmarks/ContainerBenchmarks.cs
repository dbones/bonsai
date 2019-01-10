namespace Bones.Benchmarks
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Mathematics;
    using Castle.MicroKernel.Lifestyle;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
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
        }

        [IterationSetup]
        public void Setup()
        {
            _bonesScope = _bonesContainer.CreateScope();
            _windsorScope = _windsorContainer.BeginScope();
            _autofacScope = _autofacContainer.BeginLifetimeScope();
        }
        
        [IterationCleanup]
        public void Cleanup()
        {
            _bonesScope.Dispose();
            _windsorScope.Dispose();
            _autofacScope.Dispose();
        }
        
        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _bonesContainer.Dispose();
            _windsorContainer.Dispose();
            _autofacContainer.Dispose();
        }
        

        protected abstract Bones.IModule SetupBones();
        protected abstract IWindsorInstaller SetupWindsor();
        protected abstract Autofac.Module SetupAutofac();
    }
}