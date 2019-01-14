namespace Bonsai.Tests.TestModels.Logger
{
    using System;

    /// <summary>
    ///     2nd logger with dispose
    /// </summary>
    public class LoggerWithDisposable : ILogger, IDisposable
    {
        private readonly ClassMonitor _monitor;

        public LoggerWithDisposable(ClassMonitor monitor)
        {
            _monitor = monitor;
        }
        
        public virtual void Dispose()
        {
            _monitor.ObjectDisposed(this);
        }
    }
}