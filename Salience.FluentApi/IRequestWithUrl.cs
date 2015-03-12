using System.Net;

namespace Salience.FluentApi
{
    public interface IRequestWithUrl
    {
        IRequestWithExpectedStatus Expecting(params HttpStatusCode[] acceptedStatusCodes);
    }
}