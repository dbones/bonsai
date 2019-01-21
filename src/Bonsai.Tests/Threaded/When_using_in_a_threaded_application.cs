namespace Bonsai.Tests.Threaded
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LifeStyles;
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels.Logger;
    using TestModels.Service1;

    [Subject("Container")]
    public class When_using_in_a_threaded_application
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            _container = builder.Create();
            
            _results = new List<Task<ILogger>>();
            
            for (int i = 0; i < 100; i++)
            {
            
                Task<ILogger> getService = new Task<ILogger>(() =>
                {
                    using (var scope = _container.CreateScope())
                    {
                        var service = scope.Resolve<IService>();
                        return service.Logger;
                    }   
                });

                _results.Add(getService);
                
            }
            
        };

        private Because of = () =>
        {
            //run as many threads as we can at the same time.
            foreach (var result in _results)
            {
                result.Start();
            }

            Task.WaitAll(_results.ToArray());
         
            //get the expected result
            using(var scope2 = _container.CreateScope())
            {
                _expected = scope2.Resolve<ILogger>();
            }

        };

        It should_provide_a_single_instance_of_the_logger = 
            () => PAssert.IsTrue(() => _results.All(t => t.Result == _expected));
        
        static ILogger _expected;
        static IContainer _container;
        static IList<Task<ILogger>> _results;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                builder.Register<ServiceWithCtor>().As<IService>().Scoped<Transient>();
                builder.Register<LoggerPlain>().As<ILogger>().Scoped<Singleton>();
            }
        }
    }
}