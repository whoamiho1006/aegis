using Aegis.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Aegis.Endpoints.HTTP
{
    public class Listener : IListener
    {
        private HttpListener m_Listener;
        private ConcurrentBag<IFilter<IConnection>> m_Filters;
        private Dispatcher<HttpListenerContext> m_Dispatcher;

        /// <summary>
        /// Initialize a HTTP Listener
        /// </summary>
        public Listener(params IPEndPoint[] Endpoints)
        {
            m_Listener = new HttpListener();
            m_Filters = new ConcurrentBag<IFilter<IConnection>>();
            m_Dispatcher = new Dispatcher<HttpListenerContext>();

            foreach (IPEndPoint Endpoint in Endpoints)
            {
                m_Listener.Prefixes.Add(string
                    .Join("", "http://", Endpoint, "/"));
            }

            m_Listener.Start();
            m_Listener.BeginGetContext(OnCompletion, this);
        }

        /// <summary>
        /// Completion Event of Listener.
        /// </summary>
        /// <param name="ar"></param>
        private void OnCompletion(IAsyncResult ar)
        {
            HttpListenerContext Context;

            try
            {
                Context = m_Listener.EndGetContext(ar);
                m_Listener.BeginGetContext(OnCompletion, this);
            }
            catch { return; }

            m_Dispatcher.Put(Context);
        }

        /// <summary>
        /// Test some-requests are pending or not.
        /// </summary>
        /// <returns></returns>
        public bool HasPending() => m_Dispatcher.IsEmpty;

        /// <summary>
        /// Accept request, throws exceptions if failed.
        /// </summary>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        public IRequest Accept(int Timeout)
        {
            HttpListenerContext Context;

            if (m_Dispatcher.Dispatch(Timeout, out Context))
            {
                List<IFilter<IConnection>> Filters 
                    = new List<IFilter<IConnection>>(m_Filters);

                var Connection = new Connection(Context);
                for(int i = 0; i < Filters.Count; i++)
                {
                    var Result = Filters[i].Filter(Connection);

                    switch (Result)
                    {
                        case EFilterResult.Reject:
                            Connection.Disconnect();
                            Connection = null;
                            break;

                        case EFilterResult.Retry:
                            Filters.Add(Filters[i]);
                            break;
                    }
                }

                Filters.Clear();

                if (!(Connection is null))
                {
                    
                }
            }

            return null;
        }

        /// <summary>
        /// Try accept request without exception.
        /// </summary>
        /// <param name="Timeout"></param>
        /// <param name="Request"></param>
        /// <returns></returns>
        public bool TryAccept(int Timeout, out IRequest Request) 
            => !((Request = Accept(Timeout)) is null);

        /// <summary>
        /// Co-operate with a connection filter.
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public bool With(IFilter<IConnection> Filter)
        {
            if (m_Filters.Contains(Filter))
                return false;

            m_Filters.Add(Filter);
            return true;
        }
    }
}
