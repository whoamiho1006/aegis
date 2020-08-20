using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints.HTTP
{
    /// <summary>
    /// Handler object interface.
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Handle context in sync.
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        Context Handle(Context Context);
    }
}
