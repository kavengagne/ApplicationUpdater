using System;
using System.Diagnostics;
using Updater.Helpers;
using Updater.Interfaces;

namespace Updater.Loggers
{
    public class EventLogLogger : ILogger
    {
        private const string LOG_NAME = "ApplicationUpdater";

        public EventLogLogger()
        {
            ExceptionHelper.TrySafe<Exception>(() =>
            {
                if (!EventLog.SourceExists(LOG_NAME))
                {
                    EventLog.CreateEventSource(LOG_NAME, "Application");
                }
            });
        }

        public void Log(string message)
        {
            ExceptionHelper.TrySafe<Exception>(
                () => EventLog.WriteEntry(LOG_NAME, message, EventLogEntryType.Error));
        }
    }
}