namespace Salience.FluentApi
{
    public interface IRequestWithExpectedStatus : IExecutableRequest
    {
        IExecutableRequest<T> WithContent<T>();
    }
}