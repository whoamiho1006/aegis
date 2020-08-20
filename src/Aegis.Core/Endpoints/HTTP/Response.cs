using Aegis.Endpoints.Common;

namespace Aegis.Endpoints.HTTP
{
    internal class Response : IResponse
    {
        /// <summary>
        /// Status code for response.
        /// </summary>
        public EStatusCode StatusCode { get; set; }

        /// <summary>
        /// Header collection for response.
        /// </summary>
        public HeaderCollection Headers { get; } = new HeaderCollection();

        /// <summary>
        /// Output content for response.
        /// </summary>
        public ResponseContent Output { get; set; }
    }

    
}
