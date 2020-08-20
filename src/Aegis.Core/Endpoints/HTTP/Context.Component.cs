namespace Aegis.Endpoints.HTTP
{
    public partial class Context
    {
        /// <summary>
        /// Context component.
        /// </summary>
        public class Component
        {
            /// <summary>
            /// Begins this component with context.
            /// </summary>
            /// <param name="Context"></param>
            internal void Begin(Context Context)
            {
                this.Context = Context;
                OnBegin();
            }

            /// <summary>
            /// Ends this component and removed from context.
            /// </summary>
            internal void End()
            {
                OnEnd();
                Context = null;
            }

            /// <summary>
            /// Context reference.
            /// </summary>
            public Context Context { get; private set; }

            /// <summary>
            /// Callback for begining component.
            /// </summary>
            protected virtual void OnBegin() { }

            /// <summary>
            /// Callback for ending component.
            /// </summary>
            protected virtual void OnEnd() { }
        }

        
    }
}
