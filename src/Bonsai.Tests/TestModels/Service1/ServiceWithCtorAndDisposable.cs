namespace Bonsai.Tests.TestModels.Service1
{
    using System;
    using Logger;

    /// <summary>
    ///     simple service, ctor injection (ILogger)
    /// </summary>
    public class ServiceWithCtorAndDisposable : IService, IDisposable
    {
        private readonly ClassMonitor _monitor;

        public ServiceWithCtorAndDisposable(ILogger logger, ClassMonitor monitor)
        {
            _monitor = monitor;
            Logger = logger;
        }

        public void Dispose()
        {
            _monitor.ObjectDisposed(this);
        }

        public virtual ILogger Logger { get; }
    }
}