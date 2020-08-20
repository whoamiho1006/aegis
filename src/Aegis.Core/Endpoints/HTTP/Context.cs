using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints.HTTP
{
    public partial class Context
    {
        private List<Component> m_Components;

        /// <summary>
        /// Initialize a context with request and response.
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="Response"></param>
        public Context(IRequest Request, IResponse Response)
        {
            m_Components = new List<Component>();

            this.Request = Request;
            this.Response = Response;
        }

        /// <summary>
        /// Request and its body of this context.
        /// </summary>
        public IRequest Request { get; }

        /// <summary>
        /// Response and its body for this context.
        /// </summary>
        public IResponse Response { get; }


    }
}
