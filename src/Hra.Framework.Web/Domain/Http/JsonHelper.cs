using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hra.Framework.Web.Domain.Http
{
    internal static class JsonHelper
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        static JsonHelper()
        {
            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public static T Deserialize<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString, JsonSerializerOptions);

        public static string Serialize(object @object) => JsonSerializer.Serialize(@object, JsonSerializerOptions);
    }
}
