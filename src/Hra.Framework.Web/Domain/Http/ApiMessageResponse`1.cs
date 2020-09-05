using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hra.Framework.Web.Domain.Http
{
    public class ApiMessageResponse<T> : ApiMessageResponse
    {
        public new ApiResponse<T> ToApiResponse() => new ApiResponse<T> { Errors = Errors.ToList(), Data = Data };

        public T Data { get; set; }

        public new static async Task<ApiMessageResponse<T>> FromMessage(HttpResponseMessage message)
        {
            if (message == null) return null;

            var response = new ApiMessageResponse<T>
            {
                Message = message,
                ResponseBody = await message.Content.ReadAsStringAsync().ConfigureAwait(false)
            };

            if (message.IsSuccessStatusCode)
            {
                response.Data = JsonHelper.Deserialize<T>(response.ResponseBody);
            }
            else
            {
                response.HandleErrors();
            }

            return response;
        }

    }
}
