namespace Bones.Tests.TestModels.Service1
{
    using Logger;

    /// <summary>
    ///     simple service, ctor injection (ILogger)
    /// </summary>
    public class ServiceWithCtor : IService
    {
        public ServiceWithCtor(ILogger logger)
        {
            Logger = logger;
        }

        public virtual ILogger Logger { get; }
    }
}