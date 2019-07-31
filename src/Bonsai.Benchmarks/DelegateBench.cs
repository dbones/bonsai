namespace Bonsai.Benchmarks
{
    using System;
    using System.Linq;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Mathematics;
    using FastExpressionCompiler;
    using Models;

    [InvocationCount(10000, 100)]
    [RankColumn(NumeralSystem.Stars)]
    [MemoryDiagnoser]
    public class DelegateBench
    {
        private Func<object> expressionCompiled;
        private Func<object> fastExpressionCompiled;
        private Func<object> activator;
        private Func<object> defaultCtor;
        private Func<object> reflection;
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            var loggerType = typeof(Logger);
            var loggerCtor = loggerType.GetConstructors().First();

            var repositoryType = typeof(Repository<User>);
            var repositoryCtor = repositoryType.GetConstructors().First();

            var serviceType = typeof(Service);
            var serviceCtor = serviceType.GetConstructors().First();
            
            var newLoggerExpression = System.Linq.Expressions.Expression.New(loggerCtor);
            var newRepositoryExpression = System.Linq.Expressions.Expression.New(repositoryCtor, newLoggerExpression);
            var newServiceExpression = System.Linq.Expressions.Expression.New(serviceCtor, newRepositoryExpression, newLoggerExpression);
            
            var lambdaCtor = System.Linq.Expressions.Expression.Lambda<Func<object>>(newServiceExpression);
            expressionCompiled = lambdaCtor.Compile();
            fastExpressionCompiled = lambdaCtor.CompileFast();

            activator = () => System.Activator.CreateInstance(serviceType, 
                System.Activator.CreateInstance(repositoryType, System.Activator.CreateInstance(loggerType)), 
                System.Activator.CreateInstance(loggerType));

            object[] noparams = new object[] { };
            reflection = () => serviceCtor.Invoke(new [] { repositoryCtor.Invoke(new[] { loggerCtor.Invoke(noparams) }), loggerCtor.Invoke(noparams)});

            defaultCtor = () => new Service(new Repository<User>(new Logger()), new Logger());
        }
        
        [Benchmark(Baseline = true)]
        public object Direct() => defaultCtor();
        
        [Benchmark]
        public object Expression() => expressionCompiled();
        
        [Benchmark]
        public object FastExpression() => fastExpressionCompiled();
        
        [Benchmark]
        public object Activator() => activator();
        
        [Benchmark]
        public object Reflection() => reflection();
        
       
    }
}