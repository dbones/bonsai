using System.Collections.Generic;

namespace Bonsai.Tests.TestModels.Service1
{
    public class AggregateService
    {
        public IEnumerable<IService> Services { get; }

        public AggregateService(IEnumerable<IService> services)
        {
            Services = services;
        }
    }
}