using System.Net;

namespace Salience.FluentApi
{
    public interface IRequestWithUrl : IExecutableRequest
    {
        IRequestWithExpectedStatus Expecting(HttpStatusCode expectedStatusCode, params HttpStatusCode[] otherAcceptedStatusCodes);
        IExecutableRequest<T> Expecting<T>();
    }
}