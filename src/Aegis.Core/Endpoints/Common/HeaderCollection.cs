using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aegis.Endpoints.Common
{
    public class HeaderCollection : List<IHeader>
    {
        /// <summary>
        /// Find a header instance by Name.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IHeader Find(string Name)
        {
            foreach(var Each in this)
            {
                if (Each.Attribute.Name == Name)
                    return Each;
            }

            return null;
        }

        public IHeader Find<HeaderType>()
            where HeaderType: IHeader
        {
            return Find(X => X is HeaderType);
        }

        /// <summary>
        /// Add(or set) a header instance
        /// </summary>
        /// <param name="header"></param>
        public new void Add(IHeader header)
        {
            if (header.Attribute.AllowMultiple)
                base.Add(header);

            else Set(header);
        }

        /// <summary>
        /// Set a header.
        /// </summary>
        /// <param name="header"></param>
        public void Set(IHeader header)
        {
            for(int i = 0; i < Count; i++)
            {
                IHeader Each = this[i];
                if (Each.Attribute.Name == header.Attribute.Name)
                {
                    this[i] = header;
                    return;
                }
            }

            base.Add(header);
        }

        /// <summary>
        /// Unset a header.
        /// </summary>
        /// <param name="header"></param>
        public void Unset(IHeader header)
        {
            for(int i = 0; i < Count; i++)
            {
                IHeader Each = this[i];

                if (Each.Attribute.Name == header.Attribute.Name)
                {
                    RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Import headers from .NET Core's Request implementation.
        /// </summary>
        /// <param name="Request"></param>
        internal void Import(HttpListenerRequest Request)
        {
            Clear();

            foreach(var Each in Request.Headers.AllKeys)
            {
                string Value = string.Join(": ", Each, Request.Headers[Each]);
                IHeader Instance = Header.Instantiate(ref Value);

                if (!(Instance is null)) Add(Instance);
            }
        }

        /// <summary>
        /// Export headers to .NET Core's Response implementation.
        /// </summary>
        /// <param name="Response"></param>
        internal void Export(HttpListenerResponse Response)
        {
            foreach(var Each in this)
            {
                try
                {
                    Response.AddHeader(
                        Each.Attribute.Name, Each.ToString());
                }

                catch { }
            }
        }
    }
}
