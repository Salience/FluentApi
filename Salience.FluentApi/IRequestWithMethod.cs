namespace Salience.FluentApi
{
    public interface IRequestWithMethod : IRequestWithUrl
    {
        IRequestWithUrl UsingBase(string otherBaseApiPath);
    }
}