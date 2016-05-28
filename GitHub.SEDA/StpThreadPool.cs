
using Amib.Threading;

namespace GitHub.SEDA
{
    /// <summary>
    /// SmartThreadPool based impl of IStageThreadPool
    /// </summary>
    /// <seealso cref="SmartThreadPool"/>
    public class StpThreadPool : AbstractStageThreadPool
    {
        /// <summary>
        /// The backing SmartThreadPool
        /// </summary>
        public SmartThreadPool SmartThreadPool { get; set; }

        /// <summary>
        /// A friendly Name
        /// </summary>
        public override string Name
        {
            get { return SmartThreadPool.Name; }
            set { SmartThreadPool.Name = value; }
        }

        /// <summary>
        /// Minimum threads property
        /// </summary>
        public override int MinThreads
        {
            get { return SmartThreadPool.MinThreads; }
            set { SmartThreadPool.MinThreads = value; }
        }

        /// <summary>
        /// Maximum threads property
        /// </summary>
        public override int MaxThreads
        {
            get { return SmartThreadPool.MaxThreads; }
            set { SmartThreadPool.MaxThreads = value; }
        }

        /// <summary>
        /// Gracefully shutdown
        /// </summary>
        public override void ShutDown()
        {
            SmartThreadPool.Shutdown();
        }

        /// <summary>        
        /// Queue work item to the pool. The work item
        /// will run right away if a thread is available
        /// other wise it will be queued. Exact semantics
        /// will depend on implementation        
        /// </summary>
        /// <param name="context">the stage's worker 
        /// execution context</param>
        public override void QueueWork(StageWorkerContext context)
        {
            SmartThreadPool.QueueWorkItem(StageWorkerThread, context);
        }
    }
}