namespace Bonsai.Benchmarks.Models
{
    public class Repository<T>
    {
        public Logger Logger { get; }

        public Repository(Logger logger)
        {
            Logger = logger;
        }
        
    }
}