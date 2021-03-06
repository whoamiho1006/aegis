﻿using Aegis.Endpoints.Common;

namespace Aegis.Endpoints
{
    public interface IResponse
    {
        /// <summary>
        /// Status code for response.
        /// </summary>
        EStatusCode StatusCode { get; set; }

        /// <summary>
        /// Header collection for response.
        /// </summary>
        HeaderCollection Headers { get; }

        /// <summary>
        /// Output content for response.
        /// </summary>
        ResponseContent Output { get; set; }
    }
}
