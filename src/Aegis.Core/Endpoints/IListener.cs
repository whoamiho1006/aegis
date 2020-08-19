using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints
{

    public interface IListener
    {
        /// <summary>
        /// Test some-requests are pending or not.
        /// </summary>
        /// <returns></returns>
        bool HasPending();

        /// <summary>
        /// Try accept request without exception.
        /// </summary>
        /// <param name="Timeout"></param>
        /// <param name="Request"></param>
        /// <returns></returns>
        bool TryAccept(int Timeout, out IRequest Request);

        /// <summary>
        /// Accept request, throws exceptions if failed.
        /// </summary>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        IRequest Accept(int Timeout);

        /// <summary>
        /// Co-operate with a connection filter.
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        bool With(IFilter<IConnection> Filter);
    }
}
