namespace Bones.Tests.TestModels.Service1
{
    using Logger;
    using System;

    /// <summary>
    ///     simple service, ctor injection (ILogger)
    /// </summary>
    public class ServiceWithCtor : IService
    {
        public ServiceWithCtor(ILogger logger)
        {
            if (logger == null) throw new Exception(nameof(logger));
            Logger = logger;
        }

        public virtual ILogger Logger { get; }
    }
}