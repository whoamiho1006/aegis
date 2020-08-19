using Aegis.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aegis.Workers
{
    public partial class Worker
    {
        /// <summary>
        /// Work Item.
        /// </summary>
        private class WorkItem : ObjectPool<WorkItem>, ILifeCycled
        {
            /// <summary>
            /// Initialize ObjectPool.
            /// </summary>
            static WorkItem() { MaxItems = 256; }

            /// <summary>
            /// Ensure task's completion by Dtor.
            /// </summary>
            ~WorkItem() => (this as ILifeCycled).End();

            /// <summary>
            /// Callback to be invoked.
            /// </summary>
            public WaitCallback Callback;

            /// <summary>
            /// State of Callback.
            /// </summary>
            public object State;

            /// <summary>
            /// Begin life-cycle of Work-Item.
            /// </summary>
            /// <returns></returns>
            bool ILifeCycled.Begin() => true;

            /// <summary>
            /// Finalize life-cycle of Work-Item.
            /// </summary>
            void ILifeCycled.End()
            {
                Callback = null;
                State = null;
            }
        }


    }
}
