namespace Aegis.Endpoints
{

    public enum EStatusCode
    {
        /// <summary>
        /// Succeed and Content generated.
        /// </summary>
        Okay = 200,

        /// <summary>
        /// Succeed/Allowed but, no content.
        /// </summary>
        NoContent = 204,

        /// <summary>
        /// Authorization required.
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// Forbidden access.
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// No such resource available.
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// Timed out for getting target resource accessing.
        /// </summary>
        TimedOut = 408,

        /// <summary>
        /// Duplicated access detected, so.
        /// </summary>
        Conflict = 409,

        /// <summary>
        /// Disallowed method or unimplemented method.
        /// </summary>
        MethodNotAllowed = 405,

        /// <summary>
        /// Server has error during access to resource.
        /// </summary>
        InternalServerError = 500,

        /// <summary>
        /// Target resource has being but, unimplemented.
        /// </summary>
        NotImplemented = 501,

        /// <summary>
        /// Target resource state unknown and, unavailable yet.
        /// </summary>
        Unavailable = 503
    }
}
