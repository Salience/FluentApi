namespace Salience.FluentApi
{
    public interface IFluentClient
    {
        Newtonsoft.Json.JsonSerializer Serializer { get; set; }
        IFluentClient AddTrace(ITraceWriter traceWriter);
        IFluentClient RemoveTrace(ITraceWriter traceWriter);

        IRequestWithOperation To(string description);
    }
}