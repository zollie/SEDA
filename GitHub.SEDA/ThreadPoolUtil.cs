
using System.Collections.Generic;
using System.Threading;
using Common.Logging;

namespace GitHub.SEDA
{
    /// <summary>
    /// This helps manage threads when using the 
    /// process ThreadPool from the BCL. It allows for graceful
    /// shutdowns. 
    /// </summary>
    /// <seealso cref="ThreadPool"/>
    public static class ThreadPoolUtil
    {
        /// <summary>
        /// Timeout when joining on threads
        /// </summary>
        const int JoinTimeout = 1000 * 30;

        /* logger */
        static readonly ILog Log
            = LogManager.GetLogger(typeof(ThreadPoolUtil));

        /* cache of running Threads */
        static readonly IList<Thread> Threads = new List<Thread>();

        // shutdown flag
        static volatile bool _shuttingDown;

        /// <summary>
        /// Shutdown ThreadPool Util
        /// </summary>
        public static void Shutdown()
        {
            if (_shuttingDown) return; // nother thread beat us to it                     

            lock (Threads)
            {
                if (_shuttingDown) return;
                _shuttingDown = true;
                Log.Info(typeof(ThreadPoolUtil).Name + " shutting down ...");

                Log.Info("There are " + Threads.Count + " running Threads");
                foreach (var t in Threads)
                {
                    if (!t.IsAlive) continue;

                    Log.Debug("IsBackGround = " + t.IsBackground);
                    Log.Debug("IsThreadPoolThread = " + t.IsThreadPoolThread);

                    var tid = t.ManagedThreadId;
                    if (tid == Thread.CurrentThread.ManagedThreadId)
                    {
                        Log.Info("Thread is same as current, continuing");
                        continue;
                    }

                    if (t.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        Log.Info("Interrupting Thread [" + tid + "]");
                        t.Interrupt();
                    }

                    Log.Info("Joining on Thread [" + tid + "]");
                    if (!t.Join(JoinTimeout))
                    {
                        Log.Info("Join timed out after " + JoinTimeout);
                        Log.Info("Aborting Thread [" + tid + "]");
                        t.Abort();
                    }

                    Log.Info("Thread [" + tid + "] done");
                }
            }
            Log.Info(typeof(ThreadPoolUtil).Name + " shutdown complete.");
        }

        /// <summary>
        /// Queue a work item to the Process ThreadPool, 
        /// but with some help to allow for Thread management 
        /// such as graceful shutdown and aborting
        /// </summary>
        /// <param name="callback">The callback to run</param>
        /// <param name="state">State to use in callback</param>
        public static void QueueUserWorkItem
            (WaitCallback callback, object state)
        {
            if (Log.IsDebugEnabled)
                Log.Debug("Queing work with WaitCallBack [" + callback
                          + "], and state [" + state + "]");
            if (_shuttingDown)
            {
                Log.Warn(typeof(ThreadPoolUtil).Name
                         + " is shutting down, work will be discarded");
                return;
            }

            var ctx = new WorkerContext(callback, state);
            ThreadPool.QueueUserWorkItem(WorkerThread, ctx);
        }

        /// <summary>
        /// This is the code that runs in a 
        /// working thread on the pool
        /// </summary>
        /// <param name="state">State to use in callback</param>
        private static void WorkerThread(object state)
        {
            try
            {
                lock (Threads)
                {
                    Threads.Add(Thread.CurrentThread);
                }

                var ctx = (WorkerContext)state;
                var runnableCallback = ctx.RunnableCallback;

                // delegate to original requested work
                runnableCallback.Invoke(ctx.RunnableState);
            }
            catch (ThreadInterruptedException e)
            {
                Log.Debug("Thread [" + Thread.CurrentThread.ManagedThreadId
                          + "] interruppted");
            }
            finally
            {
                lock (Threads)
                {
                    Threads.Remove(Thread.CurrentThread);
                }
            }
        }

        /// <summary>
        /// Struct holds context for working Threads
        /// </summary>
        private struct WorkerContext
        {
            public readonly WaitCallback RunnableCallback;
            public readonly object RunnableState;

            public WorkerContext(WaitCallback runnableCallback,
                                 object runnableState)
            {
                RunnableCallback = runnableCallback;
                RunnableState = runnableState;
            }
        }
    }
}