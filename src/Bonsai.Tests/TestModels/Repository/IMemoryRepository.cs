namespace Bonsai.Tests.TestModels.Repository
{
    using DataStore;

    public interface IMemoryRepository<T> : IRepository<T>
    {
        IDataStore<T> DataStore { get; }
    }
}