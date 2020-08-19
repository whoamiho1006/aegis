namespace Aegis.Utilities
{
    public interface ILifeCycled
    {
        /// <summary>
        /// Begin the object life-cycle.
        /// </summary>
        bool Begin();

        /// <summary>
        /// End the object life-cycle.
        /// </summary>
        void End();
    }
}
