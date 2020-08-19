using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Aegis.Utilities
{
    public class Dispatcher<T>
    {
        private AutoResetEvent m_ARE = new AutoResetEvent(false);
        private ConcurrentQueue<T> m_Queue = new ConcurrentQueue<T>();

        private bool m_Finalizing = false;
        private int m_Waits = 0;

        /// <summary>
        /// On Dtor, finalizing waiting loops and internal items.
        /// </summary>
        ~Dispatcher()
        {
            while (!m_Finalizing)
                m_Finalizing = true;

            while (m_Waits > 0)
            {
                m_Queue.Clear();
                m_ARE.Set();
            }
        }

        /// <summary>
        /// Push an Item to internal concurrent queue.
        /// </summary>
        /// <param name="Item"></param>
        public void Put(T Item)
        {
            if (!m_Finalizing)
            {
                m_Queue.Enqueue(Item);
                m_ARE.Set();
            }
        }

        /// <summary>
        /// Dispatch an Item for calling thread.
        /// </summary>
        /// <returns></returns>
        public virtual T Dispatch()
        {
            T Item = default;

            Interlocked.Increment(ref m_Waits);
            while (!m_Finalizing)
            {
                m_ARE.WaitOne();

                if (m_Queue.TryDequeue(out Item))
                    break;

                else if (m_Queue.Count > 0)
                    m_ARE.Set();
            }

            Interlocked.Decrement(ref m_Waits);
            return Item;
        }

        /// <summary>
        /// Try dispatch an Item for calling thread.
        /// </summary>
        /// <param name="Timeout"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        public virtual bool Dispatch(int Timeout, out T Item)
        {
            DateTime Enter = DateTime.Now;

            Interlocked.Increment(ref m_Waits);
            while (!m_Finalizing)
            {
                m_ARE.WaitOne(Timeout);

                if (m_Queue.TryDequeue(out Item))
                {
                    Interlocked.Decrement(ref m_Waits);
                    return true;
                }

                else if (m_Queue.Count > 0)
                    m_ARE.Set();

                else if (Timeout >= 0)
                {
                    Timeout -= (int)((DateTime.Now - Enter).TotalMilliseconds);
                    Enter = DateTime.Now;

                    if (Timeout < 0)
                        break;
                }
            }

            Item = default;
            Interlocked.Decrement(ref m_Waits);
            return false;
        }
    }
}
