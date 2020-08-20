using Aegis.Endpoints.Common;
using System.Collections.Generic;
using System.IO;

namespace Aegis.Endpoints
{

    public interface IRequest
    {
        /// <summary>
        /// Connection related with this request.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// Request Headers.
        /// </summary>
        HeaderCollection Headers { get; }

        /// <summary>
        /// Request method, its required behavior-type.
        /// </summary>
        EMethod Method { get; }

        /// <summary>
        /// Host where the request targets.
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Path String points to resource.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Query String contained for specifying exact resource.
        /// </summary>
        string QueryString { get; }

        /// <summary>
        /// Query KVs.
        /// </summary>
        Dictionary<string, string> Queries { get; }

        /// <summary>
        /// Contents stream. if unsuitable request, set null.
        /// </summary>
        Stream Contents { get; }
    }
}
