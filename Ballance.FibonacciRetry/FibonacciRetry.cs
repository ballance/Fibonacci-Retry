using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ballance.Retry
{
    public class FibonacciRetry
    {
        private int _delayPrevious;
        private int _delayCurrent;
        private int _delayIntervalMaximum;
        private int _delayInitial;
        private static readonly int DefaultTryCount = 5;

        public async Task Delay()
        {
            if (_delayCurrent < _delayIntervalMaximum)
            {
                _delayCurrent = _delayCurrent + _delayPrevious;
                _delayPrevious = _delayCurrent;
            }
            await Task.Delay(_delayCurrent);
        }

        /// <summary>
        /// Non-async method allows an aggregate ref list of exceptions throw during retries to be passed back to the caller.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDo"></param>
        /// <param name="exceptions"></param>
        /// <param name="tryCount"></param>
        /// <param name="delayInitial"></param>
        /// <returns></returns>
        public T Do<T>(Func<T> toDo, ref List<Exception> exceptions, int tryCount, int delayInitial = 100)
        {
            _delayInitial = delayInitial;
            _delayCurrent = delayInitial;

            do
            {
                _delayCurrent = _delayCurrent + _delayPrevious;
                _delayPrevious = _delayCurrent;
                try
                {
                    return toDo();
                }
                catch (Exception ex)
                {
                    // If an exception is thrown, add exception to list and try, try again.
                    exceptions.Add(ex);
                    tryCount--;
                    Thread.Sleep(_delayCurrent);
                }

            } while (tryCount > 0);

            return default(T);
        }

        /// <summary>
        /// Async version of Do();  Does not provide an aggregate list of exceptions to the caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDo"></param>
        /// <param name="tryCount"></param>
        /// <param name="delayInitial"></param>
        /// <returns></returns>
        public async Task<T> DoAsync<T>(Func<T> toDo, int tryCount, int delayInitial = 100)
        {
            _delayInitial = delayInitial;
            _delayCurrent = delayInitial;

            do
            {
                _delayCurrent = _delayCurrent + _delayPrevious;
                _delayPrevious = _delayCurrent;
                try
                {
                    return toDo();
                }
                catch (Exception)
                {
                    tryCount--;
                    await Task.Delay(_delayCurrent);
                }

            } while (tryCount > 0);

            return default(T);
        }

        public T Do<T>(Func<T> toDo, ref List<Exception> exceptions)
        {
            return Do(toDo, ref exceptions, DefaultTryCount);
        }

        /// <summary>
        /// If no exception ref collection is specified, throw the baby out with the bath water.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDo">Method to run</param>
        /// <returns>Result of last successful method invocation or default(T) if it does not succeed with the specified number of retries</returns>
        public T Do<T>(Func<T> toDo)
        {
            var exceptions = new List<Exception>();
            return Do(toDo, ref exceptions, DefaultTryCount);
        }

        [Obsolete]
        public void Reset()
        {
            _delayCurrent = _delayInitial;
        }
    }
}