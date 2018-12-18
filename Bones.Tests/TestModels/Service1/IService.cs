namespace Bones.Tests.TestModels.Service1
{
    using Logger;

    public interface IService
    {
        ILogger Logger { get; }
    }
}