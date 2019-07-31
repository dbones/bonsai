using System;
using Bonsai.Tests.TestModels.Logger;

namespace Bonsai.Tests.TestModels.Service1
{
    public class LazyService
    {
        public Lazy<ILogger> Logger { get; }

        public LazyService(Lazy<ILogger> logger)
        {
            Logger = logger;
        }
    }


    public class FuncService
    {
        public Func<ILogger> Logger { get; }

        public FuncService(Func<ILogger> logger)
        {
            Logger = logger;
        }
    }
}