namespace Bones.Benchmarks
{
    public class Service
    {
        public Repository<User> User { get; }
        public Logger Logger { get; }

        public Service(Repository<User> user, Logger logger)
        {
            User = user;
            Logger = logger;
        }
    }
}