using Aegis.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aegis.Workers
{
    /// <summary>
    /// Predictable Task Worker.
    /// This class provide fixed-thread work dispatching.
    /// </summary>
    public partial class Worker : IDisposable
    {
        private static ThreadLocal<Worker> m_TLS
                 = new ThreadLocal<Worker>();

        private static int m_Number = 0;

        private Dispatcher<WorkItem> m_Dispatcher;
        private bool m_KeepRunning;
        private int m_Disposing;

        /// <summary>
        /// Initialize a new predictable worker.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Priority"></param>
        public Worker(string Name = null, ThreadPriority Priority = ThreadPriority.Normal)
        {
            m_Disposing = 0;
            m_KeepRunning = true;

            m_Dispatcher = new Dispatcher<WorkItem>();
            Thread = new Thread(OnThreadMain)
            {
                IsBackground = true, Priority = Priority,
                Name = Name ?? "A-Worker #" + Interlocked.Increment(ref m_Number)
            };

            Thread.Start();
        }

        /// <summary>
        /// Thread Instance for this worker.
        /// </summary>
        public Thread Thread { get; }

        /// <summary>
        /// Get current worker if under worker environment.
        /// </summary>
        public static Worker Current => m_TLS.Value;

        /// <summary>
        /// Enqueue task into task-dispatcher.
        /// </summary>
        /// <param name="Callback"></param>
        /// <param name="State"></param>
        public bool Enqueue(WaitCallback Callback, object State)
        {
            if (m_KeepRunning)
            {
                WorkItem NewItem = WorkItem.Alloc();

                NewItem.Callback = Callback;
                NewItem.State = State;

                m_Dispatcher.Put(NewItem);
                return true;
            }

            return false;
        }

        /// <summary>
        /// On thread main, dispatches pending tasks gracefully.
        /// </summary>
        private void OnThreadMain()
        {
            m_TLS.Value = this;

            while (m_KeepRunning)
            {
                WorkItem Item = m_Dispatcher.Dispatch();

                if (!(Item is null))
                {
                    var Callback = Item.Callback;
                    var State = Item.State;
                    WorkItem.Free(Item);

                    Callback(State);
                }

                if (!m_KeepRunning)
                    m_TLS.Value = null;
            }

            m_TLS.Value = null;
        }

        /// <summary>
        /// On Dtor, try to destruct worker thread gracefully.
        /// All remained tasks will be canceled by WorkItem's implementation.
        /// </summary>
        ~Worker()
        {
            Dispose();
        }

        /// <summary>
        /// Dispose this worker if no more needed.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref m_Disposing) == 1)
            {
                while (Thread.IsAlive)
                {
                    while (m_KeepRunning)
                        m_KeepRunning = false;

                    try { Thread.Join(); }
                    catch { }
                }
            }
        }
    }
}
