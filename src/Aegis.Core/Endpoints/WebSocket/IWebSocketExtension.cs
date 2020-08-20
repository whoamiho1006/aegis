using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Aegis.Endpoints.WebSocket
{
    internal interface IWebSocketExtension
    {
        /// <summary>
        /// Handle protocol upgrade request.
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        bool Upgrade(HttpListenerContext Context);
    }
}
