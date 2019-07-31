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
}