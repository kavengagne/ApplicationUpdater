using Updater.Interfaces;

namespace Updater.Loggers
{
    public class NullLogger : ILogger
    {
        private static NullLogger _instance;
        
        public static NullLogger Instance
        {
            get { return _instance ?? (_instance = new NullLogger()); }
        }

        private NullLogger()
        {
        }
        
        public void Log(string message)
        {
        }
    }
}