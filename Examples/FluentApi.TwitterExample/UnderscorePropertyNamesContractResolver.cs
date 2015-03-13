using System.Text;
using Newtonsoft.Json.Serialization;

namespace FluentApi.TwitterExample
{
    class UnderscorePropertyNamesContractResolver : DefaultContractResolver
    {
        public UnderscorePropertyNamesContractResolver()
            : base(true)
        {
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            var sb = new StringBuilder(propertyName.Length + 5);

            foreach(var c in propertyName)
            {
                if(char.IsUpper(c) && sb.Length > 0)
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }

            return sb.ToString();
        }
    }
}