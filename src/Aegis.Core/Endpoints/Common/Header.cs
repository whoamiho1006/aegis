using Aegis.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Aegis.Endpoints.Common
{
    public interface IHeader
    {
        HeaderAttribute Attribute { get; }
    }


    /// <summary>
    /// Abstracts Header's common functionalities.
    /// </summary>
    /// <typeparam name="SelfType"></typeparam>
    public abstract class Header<SelfType> 
        : ObjectPool<SelfType>, ILifeCycled, IHeader
        where SelfType : Header<SelfType>, new()
    {
        private HeaderAttribute m_Attribute;

        /// <summary>
        /// Header.
        /// </summary>
        public Header() => m_Attribute = GetType()
            .GetCustomAttribute<HeaderAttribute>();

        /// <summary>
        /// Header Attribute.
        /// </summary>
        public HeaderAttribute Attribute => m_Attribute;

        /// <summary>
        /// Parse String into Header.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public abstract bool FromString(string Input);

        /// <summary>
        /// Initiate Header's life-cycle.
        /// </summary>
        /// <returns></returns>
        bool ILifeCycled.Begin()
        {
            return true;
        }

        /// <summary>
        /// De-initiate Header's life-cycle.
        /// </summary>
        void ILifeCycled.End()
        {
        }
    }

    /// <summary>
    /// Header Type repository.
    /// </summary>
    public sealed class Header
    {
        private static ConcurrentDictionary<HeaderAttribute, 
            (Func<object> Ctor, Action<object> Dtor)> m_HeaderTypes;

        /// <summary>
        /// Register a header type.
        /// (Non-registered header types are dropped)
        /// </summary>
        /// <typeparam name="HeaderType"></typeparam>
        public static void Register<HeaderType>()
            where HeaderType : ObjectPool<HeaderType>, new()
        {
            if (typeof(IHeader).IsAssignableFrom(typeof(HeaderType)))
            {
                var Attribute = typeof(HeaderType)
                    .GetCustomAttribute<HeaderAttribute>();

                if (Attribute is null)
                    throw new NotSupportedException(nameof(HeaderType));

                if (!m_HeaderTypes.TryAdd(Attribute, (
                    () => ObjectPool<HeaderType>.Alloc(),
                    (X) => ObjectPool<HeaderType>.Free(X as HeaderType)
                ))) throw new DuplicateNameException(nameof(HeaderType));
            }
        }

        /// <summary>
        /// Instanciate.
        /// </summary>
        /// <returns></returns>
        internal static IHeader Instantiate(ref string Header)
        {
            (Func<object> Ctor, Action<object> Dtor) Tuple;

            foreach (HeaderAttribute Each in m_HeaderTypes.Keys)
            {
                if (Header.StartsWith(Each.Name))
                {
                    if (!m_HeaderTypes.TryGetValue(Each, out Tuple))
                        return null;

                    return Tuple.Ctor() as IHeader;
                }
            }

            return null;
        }

        /// <summary>
        /// DeInstantiate.
        /// </summary>
        /// <param name="Instance"></param>
        internal static void DeInstantiate(object Instance)
        {
            if (Instance is IHeader)
            {
                var Attribute = Instance.GetType()
                    .GetCustomAttribute<HeaderAttribute>();

                if (!(Attribute is null))
                {
                    (Func<object> Ctor, Action<object> Dtor) Tuple;

                    if (m_HeaderTypes.TryGetValue(Attribute, out Tuple))
                        Tuple.Dtor(Instance);
                }
            }
        }
    }
}
