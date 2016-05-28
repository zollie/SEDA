
namespace GitHub.SEDA
{
    /// <summary>
    /// This event signals an exception was thrown
    /// somewhere in the System. 
    /// </summary>
    public class StageExceptionEvent : StageEvent
    {
        /// <summary>
        /// The execution context when the event was thrown.
        /// Having the context allows for retries. 
        /// </summary>
        public StageWorkerContext StageWorkerContext
        { get; private set; }

        /// <summary>
        /// Ctor with init
        /// </summary>
        /// <param name="context">Execution context 
        /// of StageWorker</param>
        public StageExceptionEvent(StageWorkerContext context)
        {
            StageWorkerContext = context;
        }
    }
}