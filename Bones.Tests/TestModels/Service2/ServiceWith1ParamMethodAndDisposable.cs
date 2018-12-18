namespace Bones.Tests.TestModels.Service2
{
    using System;
    using Logger;

    /// <summary>
    ///     2nd service with method inject of ILogger and has disposable
    /// </summary>
    public class ServiceWith1ParamMethodAndDisposable : IService2, IDisposable
    {
        private readonly ClassMonitor _monitor;
        private ILogger _logger;

        public ServiceWith1ParamMethodAndDisposable(ClassMonitor monitor)
        {
            _monitor = monitor;
        }

        public virtual void Dispose()
        {
            _monitor.ObjectDisposed(this);
        }

        public virtual ILogger Logger => _logger;

        public virtual void SetLogger(ILogger logger)
        {
            _logger = logger;
        }
    }
}