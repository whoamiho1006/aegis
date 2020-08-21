using Aegis.Endpoints.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Aegis.Endpoints.HTTP
{
    
    internal class Request : IRequest
    {
        /// <summary>
        /// Encapsulate Connection into Request.
        /// </summary>
        /// <param name="Connection"></param>
        public Request(IConnection Connection)
        {
            var OReq = ((this.Connection = Connection) as Connection).HLC.Request;

            int Question = (Path = OReq.Url.PathAndQuery).IndexOf('?');
            string[] QueryKVs = Question >= 0 ?
                (QueryString = Path.Substring(Question + 1)).Split('&') : null;

            Headers.Import(OReq);

            /* *
             * All request must have User Agent.
             * */
            if (Headers.Find("User-Agent") is null)
                throw new InvalidDataException("User-Agent");

            Question = Question < 0 ? Path.Length : Question;
            Path = Uri.UnescapeDataString(Path.Substring(0, Question).Trim());

            if (QueryKVs != null)
            {
                foreach (string EachKV in QueryKVs)
                {
                    int Equal = EachKV.IndexOf('=');
                    string Key = EachKV.Substring(0, Equal);

                    Queries[Key] = EachKV.Substring(Equal + 1);
                }
            }

            Host = OReq.UserHostName;
            switch(OReq.HttpMethod.ToUpper())
            {
                case "GET":
                    Method = EMethod.GET;
                    break;

                case "POST":
                    Method = EMethod.POST;
                    Contents = OReq.InputStream;
                    break;

                case "PUT":
                    Method = EMethod.PUT;
                    Contents = OReq.InputStream;
                    break;

                case "PATCH":
                    Method = EMethod.PATCH;
                    Contents = OReq.InputStream;
                    break;

                case "OPTIONS":
                    Method = EMethod.OPTIONS;
                    break;

                case "DELETE":
                    Method = EMethod.DELETE;
                    break;

                default:
                    throw new NotSupportedException(OReq.HttpMethod);
            }
        }

        /// <summary>
        /// Connection related with this request.
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Request Headers.
        /// </summary>
        public HeaderCollection Headers { get; } = new HeaderCollection();

        /// <summary>
        /// Request method, its required behavior-type.
        /// </summary>
        public EMethod Method { get; }

        /// <summary>
        /// Host where the request targets.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Path String points to resource.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Query String contained for specifying exact resource.
        /// </summary>
        public string QueryString { get; }

        /// <summary>
        /// Query KVs.
        /// </summary>
        public Dictionary<string, string> Queries { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Contents stream. if unsuitable request, set null.
        /// </summary>
        public Stream Contents { get; }
    }
}
