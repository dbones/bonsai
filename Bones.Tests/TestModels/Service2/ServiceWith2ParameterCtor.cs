namespace Bones.Tests.TestModels.Service2
{
    using Logger;
    using Repository;

    /// <summary>
    ///     has method injection which has 2 injectables
    /// </summary>
    public class ServiceWith2ParameterCtor : IService2
    {
        public ServiceWith2ParameterCtor(ILogger logger, IRepository<User> repository)
        {
            Logger = logger;
            Repository = repository;
        }

        public virtual IRepository<User> Repository { get; protected set; }


        public virtual ILogger Logger { get; protected set; }
    }
}