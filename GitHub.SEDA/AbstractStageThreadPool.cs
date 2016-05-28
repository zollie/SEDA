using System;
using Common.Logging;

namespace GitHub.SEDA
{
    /// <summary>
    /// Abstract base helper class for 
    /// IStageThreadPool implementations
    /// </summary>
    /// <see cref="IStageThreadPool"/>
    public abstract class AbstractStageThreadPool 
        : IStageThreadPool
    {
        // logger
        readonly static ILog Log = LogManager
            .GetLogger(typeof(AbstractStageThreadPool));

        /// <summary>
        /// This event is fired when an Exception is caught
        /// </summary>
        public event ExceptionHandler ExceptionCaught;

        /// <summary>
        /// A friendly Name
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Minimum threads property
        /// </summary>
        public abstract int MinThreads { get; set; }

        /// <summary>
        /// Maximum threads property
        /// </summary>
        public abstract int MaxThreads { get; set; }

        /// <summary>
        /// Gracefully shutdown
        /// </summary>
        public abstract void ShutDown();

        /// <summary>        
        /// Queue work item to the pool. The work item
        /// will run right away if a thread is available
        /// other wise it will be queued. Exact semantics
        /// will depend on implementation        
        /// </summary>
        /// <param name="context">the stage's worker 
        /// execution context</param>
        public abstract void QueueWork
            (StageWorkerContext context);


        /// <summary>
        /// This is the code that runs on a ThreadPool thread
        /// </summary>
        /// <param name="state">The state for worker</param>
        protected void StageWorkerThread(object state)
        {
            var swc = (StageWorkerContext)state;

            var cb = swc.StageWorkerCallback;


            try { cb.Invoke(swc.StageWorkerEvent); }
            catch (Exception e)
            {
                swc.Exception = e;
                OnException(swc);
            } 
        }

        /// <summary>
        /// Fires ExceptionCaught event
        /// </summary>
        /// <param name="ctx">The StageWorkerContext</param>
        private void OnException(StageWorkerContext ctx)
        {
            Log.Error("Caught exception in ThreadPool", ctx.Exception);
            if (ExceptionCaught == null) return;
            ExceptionCaught(this, ctx);
        }
    }
}
