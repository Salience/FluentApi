using System.IO;
using Newtonsoft.Json;
using RestSharp.Serializers;

namespace Salience.FluentApi
{
    internal class JsonDotNetSerializerWrapper : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        public JsonDotNetSerializerWrapper(Newtonsoft.Json.JsonSerializer serializer)
        {
            this.ContentType = "application/json";
            _serializer = serializer;
        }

        public string Serialize(object obj)
        {
            using(var stringWriter = new StringWriter())
            {
                using(var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.None;
                    jsonTextWriter.QuoteChar = '"';
                    _serializer.Serialize(jsonTextWriter, obj);
                    return stringWriter.ToString();
                }
            }
        }

        public string DateFormat { get; set; }
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string ContentType { get; set; }
    }
}