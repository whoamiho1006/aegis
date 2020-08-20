using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints
{
    public interface IKernel
    {
        /// <summary>
        /// Set this kernel to start to get context.
        /// </summary>
        void Start();

        /// <summary>
        /// Set this kernel to stop getting context.
        /// Terminating stages are programmed by Dtors.
        /// </summary>
        void Stop();
    }
}
