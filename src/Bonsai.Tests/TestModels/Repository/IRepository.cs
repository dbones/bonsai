namespace Bonsai.Tests.TestModels.Repository
{
    using Logger;

    public interface IRepository<T>
    {
        ILogger Logger { get; }
    }
}