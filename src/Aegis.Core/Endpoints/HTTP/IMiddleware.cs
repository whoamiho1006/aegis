using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aegis.Endpoints.HTTP
{
    public interface IMiddleware
    {
        /// <summary>
        /// Handle Context by Middleware Stack,
        /// If a response is to be sent, the implementer 
        /// must return the Context without calling the Next callback handler.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        Task<Context> Handle(Context Context, IMiddleware Next);
    }
}
