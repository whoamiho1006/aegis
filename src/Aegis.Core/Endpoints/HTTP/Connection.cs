using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Aegis.Endpoints.HTTP
{
    internal class Connection : IConnection
    {
        private HttpListenerContext m_Context;
        private bool m_Availability;

        /// <summary>
        /// Initialize an HTTP context into Connection.
        /// </summary>
        /// <param name="Context"></param>
        public Connection(HttpListenerContext Context)
        {
            RemoteAddress = (m_Context = Context)
                .Request.RemoteEndPoint;

            m_Availability = true;
        }

        /// <summary>
        /// HTTP Context of .NET Core.
        /// </summary>
        internal HttpListenerContext HLC => m_Context;

        /// <summary>
        /// Determines this connection is available or not.
        /// </summary>
        public bool IsAlive => m_Availability;

        /// <summary>
        /// Get RemoteAddress of this connection.
        /// </summary>
        public IPEndPoint RemoteAddress { get; }

        /// <summary>
        /// Commit Response to this connection.
        /// </summary>
        public void Commit() => Disconnect(false);

        /// <summary>
        /// Disconnect this connection.
        /// </summary>
        public void Disconnect(bool Graceful = true)
        {
            if (m_Availability)
            {
                while (m_Availability)
                    m_Availability = false;

                try
                {
                    if (Graceful)
                    {
                        m_Context.Response.StatusCode = 503;
                        m_Context.Response.StatusDescription = "Service Unavailable";
                    }

                    m_Context.Response.Close();
                }
                catch { }
            }
        }
    }
}
