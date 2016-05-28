using System;
using System.Collections.Generic;
using Common.Logging;

namespace GitHub.SEDA
{
    /// <summary>
    /// Abstract helper base for Stage impls
    /// 
    /// This is the heart to the SEDA pattern
    /// and implements the entire IStage interface
    /// </summary>
    /// <see cref="IStage"/>
    public abstract class AbstractStage : IStage
    {
        // logger
        readonly static ILog Log = LogManager
            .GetLogger(typeof(AbstractStage));


        /* The stages event handler */
        StageEventHandler _stageEventHandler;

        /// <summary>
        /// This defines the event fired from this stage. A stage
        /// can only register once. This prevents some potentially
        /// hard to find bugs. 
        /// </summary>
	    public virtual event StageEventHandler StageEvent
        {
            add
            {
                /* only register if not on the list */
                if (_stageEventHandler != null
                    && ((IList<Delegate>)_stageEventHandler
                    .GetInvocationList()).Contains(value))
                    return;

                Log.Info("Registering " + value.Target
                         + " to listen for events from " + GetType().Name);
                _stageEventHandler += value;
            }
            remove
            {   /* standard remove */
                Log.Info("Unregistering " + value.Target
                    + " from " + GetType().Name);
                _stageEventHandler -= value;
            }
        }

        /// <summary>
        /// The internal stage thread pool
        /// </summary>
        IStageThreadPool _threadPool;

        /// <summary>
        /// The stages thread pool. If you leave this null
        /// the stage will run as if RunInCallerContext is true
        /// </summary>
        /// <see cref="RunInCallerContext"/>
        public IStageThreadPool StageThreadPool
        {
            get { return _threadPool; }
            set
            {
                _threadPool = value;
                _threadPool.Name = GetType().Name + "TP";
                _threadPool.ExceptionCaught += (s, e)
                    => FireStageEvent(new StageExceptionEvent(e));
            }
        }


        /// <summary>
        /// If false, run on the Stage's ThreadPool, 
        /// else run on Thread that fired StageEventHandler
        /// </summary>
        public virtual bool RunInCallerContext { get; set; }


        /// <summary>
        /// Return the stages that are registered
        /// for this Stages StageEvent
        /// </summary>
        /// <returns>A list of registered IStages</returns>
        public IList<IStage> GetObservingStages()
        {
            IList<IStage> lst = new List<IStage>();
            if (_stageEventHandler == null)
                return lst;
            foreach (var d in _stageEventHandler.GetInvocationList())
                lst.Add((IStage)d.Target);
            return lst;
        }

        /// <summary>
        /// Comparer to place an order on calls
        /// to registered listeners during the FireStageEvent
        /// call. The name observer comes from the Observable pattern        
        /// You should probably only set this with 
        /// RunInCallerContext set to true
        /// 
        /// *A hint on implementing a Comparer, the Delegate.Target
        /// property will have the name of the registered stage. 
        /// </summary>
        public virtual IComparer<Delegate> ObserverComparer { get; set; }


        /// <summary>
        /// Stage implementations override this to do their work
        /// </summary>
        /// <param name="e">The event to handle</param>
        protected abstract void StageWorker(StageEvent e);


        /// <summary>
        /// Send an event to all registered listeners
        /// </summary>
        /// <param name="e">The event to fire</param>
        public virtual void FireStageEvent(StageEvent e)
        {
            if (ObserverComparer != null)
                FireStageEvent(e, ObserverComparer);
            else // better not to reflect if we do not have to
                _stageEventHandler(this, e);
        }

        /// <summary>
        /// Send event to registered listeners. This 
        /// overload allows you to sort the Invocation
        /// list of the event delegate. This is useful
        /// when you want to apply non-default order to the 
        /// listener call list. You should probably only 
        /// use this with RunInCallerContext set to true. 
        /// </summary>
        /// <param name="e">The event to fire</param>
        /// <param name="comparer">IComparer to sort list of 
        /// registered Delegates</param>
        /// <see cref="IComparer{T}"/>
        public virtual void FireStageEvent
            (StageEvent e, IComparer<Delegate> comparer)
        {
            Log.Debug("Firing Stage Event with comparer " + comparer);
            if (_stageEventHandler == null) return;

            var dl = _stageEventHandler.GetInvocationList();
            Array.Sort(dl, comparer);

            /* call each observer in our order */
            foreach (var d in dl)
                d.Method.Invoke(d.Target, new object[] { this, e });
        }


        /// <summary>
        /// Stage event handler. This is what listening 
        /// stages register to, e.g. in the 
        /// Spring Application Context
        /// </summary>
        /// <param name="source">Source of event</param>
        /// <param name="e">The event being fired</param>
        public virtual void StageEventHandler
            (IStage source, StageEvent e)
        {
            if (Log.IsDebugEnabled)
                Log.Debug("StageEventHandler source is ["
                    + source + "], Current Stage is ["
                    + this + "], e is [" + e + "]");
            PushEvent(e);
        }

        /// <summary>
        /// Push event into stage
        /// </summary>
        /// <param name="e">The event</param>
        public void PushEvent(StageEvent e)
        {
            PushEvent(StageWorker, e);
        }

        /// <summary>
        /// Push event to this stage. Work may run
        /// on callers thread, or a different one depending on 
        /// the RunInCallerContext property
        /// </summary>
        /// <param name="callback">Callback</param>
        /// <param name="e">Event being queued</param>
        protected void PushEvent
            (StageWorkerCallback callback, StageEvent e)
        {
            if (Log.IsDebugEnabled)
                Log.Debug("Pushing event, current Stage is ["
                    + this + "], e is [" + e + "]");


            var swc = new StageWorkerContext(callback, e);

            if (StageThreadPool == null || RunInCallerContext
                || e.RunSynchronous)
                // run in caller context
                try { callback.Invoke(e); }
                catch (Exception ex)
                {
                    swc.Exception = ex;
                    FireStageEvent(new StageExceptionEvent(swc));
                }
            else
                // push context
                StageThreadPool.QueueWork(swc);
        }
    }
}
