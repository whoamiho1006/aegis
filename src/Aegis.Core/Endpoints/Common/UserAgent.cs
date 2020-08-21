using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints.Common
{
    [Header("User-Agent")]
    public class UserAgent : Header<UserAgent>
    {
        /// <summary>
        /// Parse Input to UAString.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public override bool FromString(string Input)
        {
            int Colon = Input.IndexOf(':');

            UAString = Input.Substring(Colon + 1).Trim();
            return true;
        }

        /// <summary>
        /// User Agent Information.
        /// </summary>
        public string UAString { get; private set; }

        /// <summary>
        /// Encode this header to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => UAString;
    }
}
