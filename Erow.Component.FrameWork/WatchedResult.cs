using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Erow.Component.FrameWork
{
    public class WatchedResult
    {
        // Methods
        public WatchedResult()
        {
            this.Results = new List<WatchingResult>();
        }

        public override string ToString()
        {
            StringBuilder watcherResultBuilder = new StringBuilder();
            watcherResultBuilder.AppendFormat("ThreadId:{0}, ElapsedTIme:{1}", Thread.CurrentThread.ManagedThreadId, this.ElapsedTime);
            this.Results.ForEach(delegate (WatchingResult _) {
                watcherResultBuilder.AppendLine().Append(_.ToString());
            });
            return watcherResultBuilder.ToString();
        }

        // Properties
        public double ElapsedTime { get; set; }

        public List<WatchingResult> Results { get; private set; }

        // Nested Types
        public class WatchingResult
        {
            // Methods
            public WatchingResult(string watchingCall, long elapsedTime)
            {
                this.WatchingCall = watchingCall;
                this.ElapsedTime = elapsedTime;
            }

            public override string ToString()
            {
                return string.Format("{0} {1}", this.WatchingCall, this.ElapsedTime);
            }

            // Properties
            public long ElapsedTime { get; private set; }

            public string WatchingCall { get; private set; }
        }
    }
}