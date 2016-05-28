
using System;
using System.Collections.Generic;

namespace GitHub.SEDA
{
    /// <summary>
    /// Delegate for the stage event handler
    /// </summary>
    /// <param name="sender">source of event</param>
    /// <param name="e">The event being fired</param>
    public delegate void
        StageEventHandler(IStage sender, StageEvent e);

    /// <summary>
    /// WaitCallback delegate for queuing work
    /// </summary>
    /// <param name="e">The event being handled</param>
    public delegate void StageWorkerCallback(StageEvent e);


    /// <summary>
    /// A Stage in a Staged Event Driven Architecture (SEDA).
    /// 
    /// This is a simplified version of SEDA. The Queues are implicit
    /// as they are inside the ThreadPool.
    /// </summary>
    /// <seealso href="http://www.eecs.harvard.edu/~mdw/proj/seda/">
    /// The Stage Event Driven Architecture pattern</seealso>
    public interface IStage
    {
        /// <summary>
        /// The event fired from this stage
        /// </summary>
        event StageEventHandler StageEvent;

        /// <summary>
        /// The stages thread pool
        /// </summary>
        IStageThreadPool StageThreadPool { get; set; }

        /// <summary>
        /// If true, run on Thread that fired the 
        /// StageEventHandler, else run on seperate Thread 
        /// 
        /// By default, a Stage will run asyncronously on
        /// a seperate thread. If you want to serialize them, 
        /// set this to true
        /// </summary>
        bool RunInCallerContext { get; set; }


        /// <summary>
        /// Return the stages that are registered
        /// for this stages StageEvent
        /// </summary>
        /// <returns>A list of registered stages</returns>
        IList<IStage> GetObservingStages();


        /// <summary>
        /// Comparer to place an order on calls
        /// to registered listeners during the FireStageEvent
        /// call. name observer comes from the Observable pattern        
        /// You should probably only set this with 
        /// RunInCallerContext set to true
        /// </summary>
        IComparer<Delegate> ObserverComparer { get; set; }


        /// <summary>
        /// Send event to registered listeners.
        /// If an ObserverComparer is set this will
        /// be used to order the call list. 
        /// </summary>
        /// <param name="e">The event to fire</param>
        void FireStageEvent(StageEvent e);


        /// <summary>
        /// Send event to registered listeners. This 
        /// overload allows you to sort the Invocation
        /// list of the event delegate. This is useful
        /// when you want to apply order to the listener call
        /// list. You should probably only use this with
        /// RunInCallerContext set to true.
        /// </summary>
        /// <param name="e">The event to fire</param>
        /// <param name="comparer">Comparer to order event observers</param>
        void FireStageEvent(StageEvent e, IComparer<Delegate> comparer);


        /// <summary>
        /// Push event into the stage, the stage will then
        /// handle the event like it was fired from an event
        /// delegate. The event may run right away on same thread, 
        /// a new thread, or be queued to run when more threads
        /// are available
        /// </summary>
        /// <see cref="StageEvent"/>
        /// <see cref="RunInCallerContext"/>
        /// <see cref="StageThreadPool"/>
        /// <param name="e">The event to push</param>
        void PushEvent(StageEvent e);
    }
}