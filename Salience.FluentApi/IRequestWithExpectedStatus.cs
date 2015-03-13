namespace Salience.FluentApi
{
    public interface IRequestWithExpectedStatus : IExecutableRequest
    {
        /// <summary>
        /// Specifies the type of the expected response body.
        /// </summary>
        /// <typeparam name="T">The type of the expected response body</typeparam>
        /// <returns>This request</returns>
        IExecutableRequest<T> WithContent<T>();
    }
}