using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints.Common
{
    [Header("Content-Type")]
    public class ContentType : Header<ContentType>
    {
        /// <summary>
        /// Parse Input to Type and Encoding.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public override bool FromString(string Input)
        {
            int Colon = Input.IndexOf(':');

            string[] Props = Input.Substring(Colon + 1)
                .Split(';', StringSplitOptions.RemoveEmptyEntries);

            Type = Props[0].Trim();

            foreach(string Prop in Props)
            {
                if (Prop.ToUpper().StartsWith("CHARSET="))
                {
                    Encoding = Encoding.GetEncoding(Prop.Substring(8));
                    break;
                }
            }

            Encoding = Encoding ?? Encoding.UTF8;
            return true;
        }

        /// <summary>
        /// Type of this request content.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Encoding of this request content.
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Encode this header to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("; ", Type, 
                string.Join('=', "charset", Encoding.WebName)
            );
        }
    }
}
