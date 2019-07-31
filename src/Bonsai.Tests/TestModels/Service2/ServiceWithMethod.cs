using Bonsai.Tests.TestModels.Logger;

namespace Bonsai.Tests.TestModels.Service2
{
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