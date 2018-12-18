namespace Bones.Tests.TestModels.Repository
{
    using DataStore;

    /// <summary>
    ///     has a ctor with a generic parameter which is the same as the Repo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InMemoryRepositoryInheritsWithPropAndCtor<T> : RepositoryWithProp<T>, IMemoryRepository<T>
    {
        public InMemoryRepositoryInheritsWithPropAndCtor(IDataStore<T> dataStore)
        {
            DataStore = dataStore;
        }

        public virtual IDataStore<T> DataStore { get; }
    }
}