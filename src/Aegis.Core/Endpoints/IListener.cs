using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints
{
    public interface IListener
    {
        /// <summary>
        /// Test some-clients are pending or not.
        /// </summary>
        /// <returns></returns>
        bool HasPending();

        /// <summary>
        /// Try accept client without exception.
        /// </summary>
        /// <param name="Timeout"></param>
        /// <param name="Request"></param>
        /// <returns></returns>
        bool TryAccept(int Timeout, out IRequest Request);

        /// <summary>
        /// Accept client, throws exceptions if failed.
        /// </summary>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        IRequest Accept(int Timeout);
    }

    public interface IRequest
    {

    }
}
