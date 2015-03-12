using System.Threading.Tasks;

namespace Salience.FluentApi
{
    public interface IExecutableRequest
    {
        void Execute();
        Task ExecuteAsync();
    }

    public interface IExecutableRequest<T>
    {
        T Execute();
        Task<T> ExecuteAsync();
    }
}