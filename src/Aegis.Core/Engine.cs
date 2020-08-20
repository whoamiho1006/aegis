using Aegis.Workers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis
{
    /// <summary>
    /// Aegis Engine.
    /// </summary>
    public class Engine
    {
        private DistributedWorker m_Worker = new DistributedWorker();

        public Engine()
        {
            if (!(TlsVariables.Get<Engine>("engine") is null))
            {
                throw new InvalidOperationException(
                    "Engine instance must be only one for a thread!");
            }

            TlsVariables.Set("engine", this);
            TlsVariables.Set("worker", m_Worker);
        }
    }
}
