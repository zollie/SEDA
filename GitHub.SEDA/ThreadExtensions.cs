
using System.Threading;

namespace GitHub.SEDA
{
    /// <summary>
    /// Extension methods for Thread
    /// </summary>    
    /// <seealso cref="Thread"/>
    public static class ThreadExtensions
    {
        /// <summary>
        /// Returns true of this Thread is running
        /// inside a Stage
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsThreadPoolThread(this Thread t)
        {
            return false;
        }
    }
}