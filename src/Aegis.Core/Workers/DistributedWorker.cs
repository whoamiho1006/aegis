using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Aegis.Workers
{
    /// <summary>
    /// This worker class implements task distribution between multiple workers.
    /// </summary>
    public class DistributedWorker : IWorker
    {
        private Queue<Worker> 
            m_Front = new Queue<Worker>(),
            m_Back = new Queue<Worker>();

        /// <summary>
        /// Parent worker instance.
        /// </summary>
        public IWorker Parent { get; }

        /// <summary>
        /// Thread Instance for this worker.
        /// This will always return null.
        /// </summary>
        public Thread Thread { get; } = null;

        /// <summary>
        /// Increase worker capability.
        /// </summary>
        /// <param name="Workers"></param>
        public void Increase(int Workers)
        {
            lock (m_Back)
            {
                while (--Workers >= 0)
                    m_Back.Enqueue(new Worker());
            }
        }

        /// <summary>
        /// Enqueue task into task-dispatcher.
        /// (this distribute tasks in round-robin rotator by two queue)
        /// </summary>
        /// <param name="Callback"></param>
        /// <param name="State"></param>
        public bool Enqueue(WaitCallback Callback, object State)
        {
            while (true)
            {
                lock (m_Front)
                {
                    if (m_Front.Count > 0)
                    {
                        var Worker = m_Front.Dequeue();

                        Worker.Enqueue(Callback, State);
                        m_Back.Enqueue(Worker);

                        return Worker.Enqueue(Callback, State);
                    }

                    else if(m_Back.Count <= 0)
                                break;

                    while (m_Back.Count > 0)
                        m_Front.Enqueue(m_Back.Dequeue());
                }
            }

            return false;
        }
    }
}
