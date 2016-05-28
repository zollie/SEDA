
using System;
using System.Text;

namespace GitHub.SEDA
{
    /// <summary>
    /// Holds the execution context of a stage worker 
    /// thread that can run on a IStageThreadPool. Similar 
    /// to a Continuation, enables things like retry.
    /// </summary>
    /// <seealso cref="IStageThreadPool"/>
    /// <seealso href="http://en.wikipedia.org/wiki/Continuation">
    /// Continuations</seealso>
    public class StageWorkerContext// : ISerializable
    {
        /// <summary>
        /// The Worker Callback
        /// </summary>
        public StageWorkerCallback StageWorkerCallback
        { get; internal set; }

        /// <summary>
        /// State for the callback
        /// </summary>
        public StageEvent StageWorkerEvent
        { get; internal set; }


        /// <summary>
        /// Holds any exception thrown by worker, 
        /// may be null
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Number of times this wokrer has been 
        /// retried after Exception
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Ctor with init
        /// </summary>
        /// <param name="cb">The worker callback</param>
        /// <param name="e">The state used by worker</param>
        public StageWorkerContext
            (StageWorkerCallback cb, StageEvent e)
        {
            StageWorkerCallback = cb;
            StageWorkerEvent = e;
        }

        /// <summary>
        /// ToString overide
        /// </summary>
        /// <returns>String representation of 
        /// the execution context</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(GetType().Name)
            .Append(" {StageWorkerCallback = ").Append(StageWorkerCallback)
            .Append(", StageWorkerEvent = ").Append(StageWorkerEvent);
            if (Exception != null)
            {
                sb.Append(", Exception = ").Append(Exception.Message)
                    .Append(", RetryCount = " + RetryCount);
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
