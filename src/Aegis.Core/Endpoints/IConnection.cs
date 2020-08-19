using System.Net;

namespace Aegis.Endpoints
{
    public interface IConnection
    {
        /// <summary>
        /// Determines this connection is available or not.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Get RemoteAddress of this connection.
        /// </summary>
        IPEndPoint RemoteAddress { get; }

        /// <summary>
        /// Disconnect
        /// </summary>
        void Disconnect(bool Graceful = true);
    }
}
