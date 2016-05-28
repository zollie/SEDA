using System;

namespace GitHub.SEDA
{
    /// <summary>
    /// A typed Stage event args 
    /// </summary>
    public class StageEvent : EventArgs
    {
        /// <summary>
        /// If true, event should run 
        /// synchronously in the caller context
        /// </summary>
        public bool RunSynchronous { get; set; }

        /// <summary>
        /// Default ctor
        /// </summary>
        protected StageEvent() { }

        /// <summary>
        /// Ctor with init
        /// </summary>
        /// <param name="runSynchronous">
        /// <see cref="RunSynchronous"/>
        /// </param>
        protected StageEvent(bool runSynchronous)
        {
            RunSynchronous = runSynchronous;
        }
    }
}