namespace Salience.FluentApi
{
    public interface IEmptyRequest
    {
        /// <summary>
        /// Initializes a new request by describing what it does.
        /// This description will appear in the traces.
        /// </summary>
        /// <param name="operation">The operation being performed by the request, e.g. "update a status"</param>
        /// <returns></returns>
        IRequestWithOperation To(string operation);
    }
}