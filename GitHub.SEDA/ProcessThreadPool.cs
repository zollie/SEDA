
using System.Threading;

namespace GitHub.SEDA
{
    /// <summary>
    /// IStageThreadPool backed by the default
    /// process ThreadPool from the BCL
    /// </summary>
    /// <seealso cref="ThreadPool"/>
    public class ProcessThreadPool : AbstractStageThreadPool
    {
        /// <summary>
        /// A friendly Name
        /// </summary>
        public override string Name
        {
            get { return GetType().Name; }
            set { } // noop
        }

        /// <summary>
        /// Minimum threads property
        /// </summary>
        public override int MinThreads
        {
            get
            {
                int workerThreads, cpThreads;
                ThreadPool.GetMinThreads
                    (out workerThreads, out cpThreads);
                return workerThreads;
            }
            set
            {
                int workerThreads, cpThreads;
                ThreadPool.GetMinThreads
                    (out workerThreads, out cpThreads);
                ThreadPool.SetMinThreads(value, cpThreads);
            }
        }

        /// <summary>
        /// Maximum threads property
        /// </summary>
        public override int MaxThreads
        {
            get
            {
                int workerThreads, cpThreads;
                ThreadPool.GetMaxThreads
                    (out workerThreads, out cpThreads);
                return workerThreads;
            }
            set
            {
                int workerThreads, cpThreads;
                ThreadPool.GetMaxThreads
                    (out workerThreads, out cpThreads);
                ThreadPool.SetMaxThreads(value, cpThreads);
            }
        }

        /// <summary>
        /// Gracefully shutdown
        /// </summary>
        public override void ShutDown()
        {
            ThreadPoolUtil.Shutdown();
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
            ThreadPoolUtil.QueueUserWorkItem(StageWorkerThread, context);
        }
    }
}