using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading;

namespace Aegis.Workers
{
    /// <summary>
    /// TLS Variables.
    /// </summary>
    public class TlsVariables
    {
        private static ThreadLocal<Dictionary<string, object>> m_Variables
            = new ThreadLocal<Dictionary<string, object>>(
                () => new Dictionary<string, object>());

        /// <summary>
        /// Set TLS Variable.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public static void Set(string Name, object Value) 
            => m_Variables.Value[Name] = Value;

        /// <summary>
        /// Get TLS Variable by Name.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static TObject Get<TObject>(string Name)
            where TObject : class
        {
            try { return m_Variables.Value[Name] as TObject; }
            catch { }

            return default;
        }

        /// <summary>
        /// Export TLS Variables of this thread.
        /// </summary>
        /// <returns></returns>
        public static (string Name, object Data)[] Export()
        {
            List<(string Name, object Data)> Items = new List<(string Name, object Data)>();

            foreach (string Name in m_Variables.Value.Keys)
                Items.Add((Name, m_Variables.Value[Name]));

            return Items.ToArray();
        }

        /// <summary>
        /// Import TLS Variables for this thread.
        /// </summary>
        /// <param name="Items"></param>
        public static void Import((string Name, object Data)[] Items)
        {
            foreach(var Each in Items)
                Set(Each.Name, Each.Data);
        }

        /// <summary>
        /// Unset TLS Variable.
        /// </summary>
        /// <param name="Name"></param>
        public static void Unset(string Name)
        {
            try { m_Variables.Value.Remove(Name); } 
            catch { }
        }
    }
}
