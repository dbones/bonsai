namespace Bones.Tests.TestModels.Repository
{
    using Logger;

    /// <summary>
    ///     generic type, with property injection of the logger
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RepositoryWith1ParamProp<T> : IRepository<T>
    {
        public virtual ILogger Logger { get; set; }
    }
}