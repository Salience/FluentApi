using RestSharp;

namespace Salience.FluentApi
{
    public interface IFluentClient
    {
        /// <summary>
        /// The JSON serializer used to serialize request bodies and deserialize response bodies.
        /// </summary>
        Newtonsoft.Json.JsonSerializer Serializer { get; set; }

        /// <summary>
        /// Sets the requests authenticator.
        /// </summary>
        /// <param name="authenticator">A RestSharp authenticator</param>
        /// <returns>This client. Useful for chaining operations.</returns>
        IFluentClient SetAuthenticator(IAuthenticator authenticator);
        
        /// <summary>
        /// Registers a trace writer to use for tracing requests and responses.
        /// </summary>
        /// <param name="traceWriter">The new trace writer</param>
        /// <returns>This client. Useful for chaining operations.</returns>
        IFluentClient AddTrace(ITraceWriter traceWriter);

        /// <summary>
        /// Unregisters a trace writer.
        /// </summary>
        /// <param name="traceWriter">The trace writer to remove</param>
        /// <returns>This client. Useful for chaining operations.</returns>
        IFluentClient RemoveTrace(ITraceWriter traceWriter);

        /// <summary>
        /// Initializes a new request by describing what it does.
        /// This description will appear in the traces.
        /// </summary>
        /// <param name="operation">The operation being performed by the request, e.g. "update a status"</param>
        /// <returns></returns>
        IRequestWithOperation To(string operation);
    }
}