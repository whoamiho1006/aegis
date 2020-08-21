using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints.Common
{
    [Header("X-Request-With")]
    public class XRequestWith : Header<XRequestWith>
    {
        private static char[] Delimiters = new char[] { ';', ' ' };

        /// <summary>
        /// Parse Input to Value and Options.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public override bool FromString(string Input)
        {
            int Colon = Input.IndexOf(':');
            string[] Values = Input.Substring(Colon + 1).Trim().Split(
                Delimiters, StringSplitOptions.RemoveEmptyEntries);

            Value = Values[0].Trim();
            foreach (string Each in Values)
            {
                int Equal = Each.IndexOf('=');
                if (Equal > 0)
                {
                    string Key = Each.Substring(0, Equal).Trim();
                    Options[Key] = Each.Substring(Equal + 1).Trim();
                }
            }

            return true;
        }

        /// <summary>
        /// X-Request-With Value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// X-Request-With Options
        /// </summary>
        public Dictionary<string, string> Options { get; private set; }

        /// <summary>
        /// Encode this header to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value;
    }
}
