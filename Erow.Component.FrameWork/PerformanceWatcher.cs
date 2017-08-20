using System;
using System.Diagnostics;

namespace Erow.Component.FrameWork
{
    public class PerformanceWatcher
    {
        // Fields
        [ThreadStatic]
        private static DateTime? startTime;
        [ThreadStatic]
        private static WatchedResult watchedResult;

        // Methods
        private PerformanceWatcher()
        {
        }

        public static PerformanceWatcher StartNewWatcher()
        {
            startTime = new DateTime?(DateTime.Now);
            watchedResult = new WatchedResult();
            return new PerformanceWatcher();
        }

        public WatchedResult StopWatcher()
        {
            WatchedResult result = new WatchedResult();
            if (startTime.HasValue && (watchedResult != null))
            {
                TimeSpan span = (TimeSpan)(DateTime.Now - startTime.Value);
                result.ElapsedTime = span.TotalMilliseconds;
                result.Results.AddRange(watchedResult.Results);
            }
            startTime = null;
            watchedResult = null;
            return result;
        }

        public static void Watch(string watchingCall, Action action)
        {
            if (string.IsNullOrEmpty(watchingCall) || !startTime.HasValue)
            {
                action();
            }
            else
            {
                Stopwatch stopwatch = new Stopwatch();
                try
                {
                    stopwatch.Start();
                    action();
                }
                finally
                {
                    stopwatch.Stop();
                    if (watchedResult == null)
                    {
                        watchedResult = new WatchedResult();
                    }
                    watchedResult.Results.Add(new WatchedResult.WatchingResult(watchingCall, stopwatch.ElapsedMilliseconds));
                }
            }
        }

        public static TResult Watch<TResult>(string watchingCall, Func<TResult> func)
        {
            TResult local;
            if (string.IsNullOrEmpty(watchingCall) || !startTime.HasValue)
            {
                return func();
            }
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                local = func();
            }
            finally
            {
                stopwatch.Stop();
                if (watchedResult == null)
                {
                    watchedResult = new WatchedResult();
                }
                watchedResult.Results.Add(new WatchedResult.WatchingResult(watchingCall, stopwatch.ElapsedMilliseconds));
            }
            return local;
        }

        public static void Watch(string watchingCall, long elapsedTime)
        {
            if (startTime.HasValue && !string.IsNullOrEmpty(watchingCall))
            {
                watchedResult.Results.Add(new WatchedResult.WatchingResult(watchingCall, elapsedTime));
            }
        }
    }
}






