using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints.HTTP
{
    internal class Request : IRequest
    {
        private IConnection m_Connection;

        /// <summary>
        /// Encapsulate Connection into Request.
        /// </summary>
        /// <param name="Connection"></param>
        public Request(IConnection Connection) => m_Connection = Connection;


    }
}
