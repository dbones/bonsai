namespace Bonsai.Tests.TestModels.Service2
{
    using System;
    using Logger;

    /// <summary>
    ///     service has Lazy parameters
    /// </summary>
    public class ServiceWithLazyConstructor : IService2
    {
        private readonly Lazy<ILogger> _logger;

        public ServiceWithLazyConstructor(Lazy<ILogger> logger)
        {
            _logger = logger;
        }

        public ILogger Logger => _logger.Value;
    }
}