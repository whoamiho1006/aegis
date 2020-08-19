using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aegis
{
    internal static class InternalExtensions
    {
        /// <summary>
        /// Create this directory if not existed.
        /// </summary>
        /// <param name="Directory"></param>
        public static void CreateIfNotExisted(this DirectoryInfo Directory)
        {
            if (!Directory.Exists)
                Directory.Create();
        }

        /// <summary>
        /// Serialize this object to JSON.
        /// </summary>
        /// <param name="Any"></param>
        /// <returns></returns>
        public static string ToJSON(this object Any) => JsonConvert.SerializeObject(Any);

        /// <summary>
        /// De-Serialize this json to object.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static TObject FromJson<TObject>(this string Json) => JsonConvert.DeserializeObject<TObject>(Json);
    }
}
