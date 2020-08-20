using Aegis.Endpoints;
using Aegis.Workers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Aegis
{
    /// <summary>
    /// Aegis Engine.
    /// </summary>
    public class Engine
    {
        private ManualResetEvent m_Event = new ManualResetEvent(false);
        private List<IKernel> m_Kernels = new List<IKernel>();

        /// <summary>
        /// Initialize an Engine instance.
        /// </summary>
        public Engine()
        {
            if (!(TlsVariables.Get<Engine>("engine") is null))
            {
                throw new InvalidOperationException(
                    "Engine instance must be only one for a thread!");
            }

            TlsVariables.Set("worker", Worker);
            TlsVariables.Set("engine", this);
        }

        /// <summary>
        /// Distributed Worker Instance.
        /// Initially Instantiated workers are `Core` * 2.
        /// </summary>
        public DistributedWorker Worker { get; } = new DistributedWorker();

        /// <summary>
        /// Make this engine to co-operate with Feature Kernels.
        /// </summary>
        /// <param name="Kernel"></param>
        /// <returns></returns>
        public Engine Use(IKernel Kernel)
        {
            lock (m_Kernels)
                m_Kernels.Add(Kernel);

            return this;
        }

        /// <summary>
        /// Starts the engine.
        /// </summary>
        public void Start()
        {
            Worker.Increase(Environment.ProcessorCount * 2 - 1);

            foreach (IKernel Kernel in m_Kernels)
                Kernel.Start();

            m_Event.WaitOne();
        }

        /// <summary>
        /// Stop the engine.
        /// </summary>
        public void Stop()
        {
            foreach (IKernel Kernel in m_Kernels)
                Kernel.Stop();

            m_Event.Set();
        }
    }
}
