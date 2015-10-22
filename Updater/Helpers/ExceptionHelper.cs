using System;
using Updater.Interfaces;
using Updater.Loggers;

namespace Updater.Helpers
{
    public static class ExceptionHelper
    {
        private static ILogger _logger = NullLogger.Instance;

        public static bool TrySafe<TException>(Action methodToExecute, Action<TException> methodOnError = null)
            where TException : Exception
        {
            try
            {
                methodToExecute.Invoke();
                return true;
            }
            catch (TException ex)
            {
                if (methodOnError != null)
                {
                    TrySafe<Exception>(() => _logger.Log(ex.ToString()));
                    TrySafe<TException>(() => methodOnError.Invoke(ex));
                }
                return false;
            }
        }


        public static bool TrySafe<TException>(Action methodToExecute, Action methodOnError)
            where TException : Exception
        {
            Action<TException> toExecSecond = e => { };
            if (methodOnError != null)
            {
                toExecSecond = e => methodOnError();
            }
            return TrySafe(methodToExecute, toExecSecond);
        }


        public static TReturn TrySafe<TException, TReturn>(Func<TReturn> methodToExecute,
                                                           Func<TReturn> methodOnError = null)
            where TException : Exception
        {
            try
            {
                return methodToExecute.Invoke();
            }
            catch (TException ex)
            {
                if (methodOnError != null)
                {
                    TrySafe<Exception>(() => _logger.Log(ex.ToString()));
                    return TrySafe<TException, TReturn>(methodOnError.Invoke);
                }
                return default(TReturn);
            }
        }


        public static void SetLogger(ILogger logger)
        {
            if (logger != null)
            {
                _logger = logger;
            }
        }
    }
}