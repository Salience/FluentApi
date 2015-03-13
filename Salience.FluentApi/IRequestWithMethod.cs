namespace Salience.FluentApi
{
    public interface IRequestWithMethod : IRequestWithUrl
    {
        /// <summary>
        /// Specifies the base path to use for the specified ressource.
        /// </summary>
        /// <param name="otherBaseApiPath">The API base to use</param>
        /// <returns>This request</returns>
        IRequestWithUrl UsingBase(string otherBaseApiPath);
    }
}