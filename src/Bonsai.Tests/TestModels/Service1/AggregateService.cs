namespace Bonsai.Tests.TestModels
{
    using System;
    using System.Collections.Generic;
    using DataStore;
    using Logger;
    using Service1;

    public class AggregateService
    {
        public IEnumerable<IService> Services { get; }

        public AggregateService(IEnumerable<IService> services)
        {
            Services = services;
        }
    }

    public class LazyService
    {
        public Lazy<ILogger> Logger { get; }

        public LazyService(Lazy<ILogger> logger)
        {
            Logger = logger;
        }
    }
}