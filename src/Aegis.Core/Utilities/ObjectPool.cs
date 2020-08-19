using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Aegis.Utilities
{
    public class ObjectPool<T> where T : class, new()
    {
        private static ConcurrentQueue<T> m_Objects = new ConcurrentQueue<T>();

        /// <summary>
        /// Count of Maximum Pooled Items.
        /// </summary>
        public static int MaxItems { get; set; } = 128;

        /// <summary>
        /// Allocate an item from pool.
        /// </summary>
        /// <returns></returns>
        public static T Alloc()
        {
            T Item = null;

            while (true)
            {
                while (m_Objects.Count > 0)
                {
                    if (m_Objects.TryDequeue(out Item))
                        break;
                }

                if (Item is null)
                    Item = new T();

                if ((Item is ILifeCycled) && 
                   !(Item as ILifeCycled).Begin())
                    continue;

                return Item;
            }
        }

        /// <summary>
        /// De-allocate an item into pool.
        /// </summary>
        /// <param name="Item"></param>
        public static void Free(T Item)
        {
            if (Item is ILifeCycled)
                (Item as ILifeCycled).End();

            if (m_Objects.Count < MaxItems)
                m_Objects.Enqueue(Item);

            else if (Item is IDisposable)
                (Item as IDisposable).Dispose();
        }
    } 
}
