namespace Bones.Tests.TestModels.Service2
{
    using Logger;
    using Repository;

    /// <summary>
    ///     has method injection which has 2 injectables
    /// </summary>
    public class ServiceWith2ParameterMethod : IService2
    {
        public virtual IRepository<User> Repository { get; protected set; }

        public virtual ILogger Logger { get; protected set; }

        public virtual void SetDependencies(ILogger logger, IRepository<User> repository)
        {
            Logger = logger;
            Repository = repository;
        }
    }
}