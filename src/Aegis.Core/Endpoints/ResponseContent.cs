using System;
using System.IO;
using System.Text;

namespace Aegis.Endpoints
{
    public struct ResponseContent
    {
        /// <summary>
        /// Mime type of content.
        /// </summary>
        public string MimeType;

        /// <summary>
        /// Content encoding.
        /// </summary>
        public Encoding Encoding;

        /// <summary>
        /// Content stream.
        /// </summary>
        public Stream Content;

        /// <summary>
        /// Content offset.
        /// Note: Offset=0, Length=0 means entire content from current.
        /// </summary>
        public long Offset;

        /// <summary>
        /// Content length.
        /// Note: Offset=0, Length=0 means entire content from current.
        /// </summary>
        public long Length;

        /// <summary>
        /// Close method for finalizing content stream.
        /// </summary>
        public Action<Stream> Close;
    }
}
