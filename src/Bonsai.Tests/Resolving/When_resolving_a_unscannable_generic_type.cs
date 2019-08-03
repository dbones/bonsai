using Bonsai.Tests.TestModels.DataStore;

namespace Bonsai.Tests.Resolving
{
    using Machine.Specifications;
    using PowerAssert;
    using Registry;
    using TestModels;

    [Subject("Container")]
    public class When_resolving_a_unscannable_generic_type
    {
        Establish context = () => {
            var builder = new ContainerBuilder();
            builder.SetupModules(new RegisterContracts());
            var container = builder.Create();
            
            _subject = container.CreateScope();
        };

        Because of = () => _service = _subject.Resolve<IDataStore<User>>();

        It should_provide_an_instance = () => PAssert.IsTrue(() => _service != null);
        It should_provide_an_instance_which_is_associated_with_the_registration = 
            () => PAssert.IsTrue(() => _service is DataStorePlain<User>);
        
        static IScope _subject;
        static IDataStore<User> _service;
        
        class RegisterContracts : IModule
        {
            public void Setup(ContainerBuilder builder)
            {
                //TODO figure out the the syntax 
                builder.Register(typeof(DataStorePlain<>)).As(typeof(IDataStore<>));
            }
        }
    }
}