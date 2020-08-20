using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Workers.Tasks
{
    public class FutureSource
    {
        /// <summary>
        /// Initialize a future source.
        /// </summary>
        public FutureSource()
        {
            Future = new Future(true, false);
        }

        /// <summary>
        /// Future instance.
        /// </summary>
        public Future Future { get; }

        /// <summary>
        /// Set completion for this future.
        /// </summary>
        public void SetCompleted()
        {
            lock (Future)
            {
                if (Future.m_Completion <= 0)
                    Future.m_Completion = 1;
            }
        }

        /// <summary>
        /// Set faulted for this future.
        /// </summary>
        /// <param name="e"></param>
        public void SetFaulted(Exception e)
        {
            lock (Future)
            {
                if (Future.m_Completion <= 0)
                {
                    Future.m_Exception = e;
                    Future.m_Completion = 1;
                }
            }
        }
    }

    public class FutureSource<TResult>
    {
        /// <summary>
        /// Initialize a future source.
        /// </summary>
        public FutureSource()
        {
            Future = new Future<TResult>(true, false);
        }

        /// <summary>
        /// Future instance.
        /// </summary>
        public Future<TResult> Future { get; }

        /// <summary>
        /// Set completion for this future.
        /// </summary>
        public void SetCompleted(TResult Result)
        {
            lock (Future)
            {
                if (Future.m_Completion <= 0)
                {
                    Future.m_Result = Result;
                    Future.m_Completion = 1;
                }
            }
        }

        /// <summary>
        /// Set faulted for this future.
        /// </summary>
        /// <param name="e"></param>
        public void SetFaulted(Exception e)
        {
            lock (Future)
            {
                if (Future.m_Completion <= 0)
                {
                    Future.m_Exception = e;
                    Future.m_Completion = 1;
                }
            }
        }
    }
}
