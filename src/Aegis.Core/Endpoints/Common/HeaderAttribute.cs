using System;

namespace Aegis.Endpoints.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HeaderAttribute : Attribute
    {
        public HeaderAttribute(string Name, bool AllowMultiple = false)
        {
            this.Name = Name;
            this.AllowMultiple = AllowMultiple;
        }

        /// <summary>
        /// Name of this header.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Whether multiple instances are allowed.
        /// </summary>
        public bool AllowMultiple { get; }
    }
}
