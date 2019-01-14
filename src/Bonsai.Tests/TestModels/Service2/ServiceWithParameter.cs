namespace Bonsai.Tests.TestModels.Service2
{
    using Logger;

    /// <summary>
    ///     service has parameter injection
    /// </summary>
    public class ServiceWithParameter : IService2
    {
        public ILogger Logger { get; set; }
    }
    
    /// <summary>
    ///     service has Method injection
    /// </summary>
    public class ServiceWithMethod : IService2
    {
        public ILogger Logger { get; private set; }

        public void SetLogger(ILogger logger)
        {
            Logger = logger;
        }
    }
}