namespace Aegis.Endpoints
{
    public enum EFilterResult
    {
        /// <summary>
        /// Accept.
        /// </summary>
        Accept,

        /// <summary>
        /// Reject.
        /// </summary>
        Reject,

        /// <summary>
        /// Retry.
        /// </summary>
        Retry
    }

    public interface IFilter<T> where T: class
    {
        /// <summary>
        /// Filter an instance.
        /// </summary>
        /// <param name="Instance"></param>
        /// <returns></returns>
        EFilterResult Filter(T Instance);
    }
}
