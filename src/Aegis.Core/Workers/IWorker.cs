using System.Threading;

namespace Aegis.Workers
{
    public interface IWorker
    {
        /// <summary>
        /// Parent worker instance.
        /// </summary>
        IWorker Parent { get; }

        /// <summary>
        /// Thread Instance for this worker.
        /// (null means multiple thread active or nothing)
        /// </summary>
        Thread Thread { get; }

        /// <summary>
        /// Enqueue task into task-dispatcher.
        /// </summary>
        /// <param name="Callback"></param>
        /// <param name="State"></param>
        bool Enqueue(WaitCallback Callback, object State);

    }
}