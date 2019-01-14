namespace Bones.Tests.TestModels.Logger
{
    public class LoggerWithCtor : ILogger
    {
        public LoggerWithCtor(ILogAppender logAppender)
        {
            LogAppender = logAppender;
        }

        public ILogAppender LogAppender { get; }
    }
}