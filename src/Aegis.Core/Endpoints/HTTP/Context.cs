using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints.HTTP
{
    public partial class Context
    {
        private List<Component> m_Components;
        
        /// <summary>
        /// Initialize a context with request and response.
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="Response"></param>
        public Context(IRequest Request, IResponse Response)
        {
            m_Components = new List<Component>();

            this.Request = Request;
            this.Response = Response;
        }

        /// <summary>
        /// Request and its body of this context.
        /// </summary>
        public IRequest Request { get; }

        /// <summary>
        /// Response and its body for this context.
        /// </summary>
        public IResponse Response { get; }

        /// <summary>
        /// Get component by its exact type.
        /// </summary>
        /// <typeparam name="ComponentType"></typeparam>
        /// <param name="AllowNew"></param>
        /// <returns></returns>
        public ComponentType GetComponent<ComponentType>(bool AllowNew = true)
            where ComponentType : Component, new()
        {
            lock (m_Components)
            {
                ComponentType Component = m_Components.Find(
                    X => X is ComponentType) as ComponentType;

                if (AllowNew && Component is null)
                {
                    m_Components.Add(Component = new ComponentType());
                    Component.Begin(this);
                }

                return Component;
            }
        }

        /// <summary>
        /// Get component by its type.
        /// </summary>
        /// <param name="ComponentType"></param>
        /// <param name="AllowNew"></param>
        /// <returns></returns>
        public Component GetComponent(Type ComponentType, bool AllowNew = true)
        {
            if (typeof(Component).IsAssignableFrom(ComponentType))
            {
                lock (m_Components)
                {
                    Component Component = m_Components.Find(
                        X => ComponentType.IsAssignableFrom(X.GetType()));

                    if (AllowNew && Component is null)
                    {
                        var Ctor = ComponentType.GetConstructor(Type.EmptyTypes);

                        if (!(Ctor is null) &&
                            !((Component = Ctor.Invoke(new object[0]) as Component) is null))
                        {
                            m_Components.Add(Component);
                            Component.Begin(this);
                        }
                    }

                    return Component;
                }
            }

            return null;
        }
    }
}
