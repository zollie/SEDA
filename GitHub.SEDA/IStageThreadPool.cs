
namespace GitHub.SEDA
{
    /// <summary>
    /// Delegate for an handling an Exception event
    /// </summary>
    /// <param name="source">source that caught exception</param>
    /// <param name="context">The execution context of 
    /// the terminating thread</param>
    public delegate void ExceptionHandler
        (IStageThreadPool source, StageWorkerContext context);

    /// <summary>
    /// Interface for a Stage ThreadPool. This allows for 
    /// swapping ThreadPool implementations. A Stage only
    /// needs some basic ThreadPool primitives defined
    /// in this interface
    /// </summary>
    public interface IStageThreadPool
    {
        /// <summary>
        /// The Exception event. Fired when a worker
        /// terminates due to exception
        /// </summary>
        event ExceptionHandler ExceptionCaught;

        /// <summary>
        /// A friendly Name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Minimum threads property
        /// </summary>
        int MinThreads { get; set; }

        /// <summary>
        /// Maximum threads property
        /// </summary>
        int MaxThreads { get; set; }

        /// <summary>
        /// Gracefully shutdown
        /// </summary>
        void ShutDown();


        /// <summary>        
        /// Queue work to the pool. The work item
        /// will run right away if a thread is available
        /// other wise it will be queued. Exact semantics
        /// will depend on implementation        
        /// </summary>
        /// <param name="context">the stage's worker 
        /// execution context</param>
        void QueueWork(StageWorkerContext context);

    }
}