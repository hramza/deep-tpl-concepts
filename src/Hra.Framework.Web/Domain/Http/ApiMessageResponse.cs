using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Hra.Framework.Web.Domain.Http
{
    public class ApiMessageResponse
    {
        public HttpResponseMessage Message { get; set; }

        public string ResponseBody { get; set; }

        public IEnumerable<ApiError> Errors { get; set; }

        public ApiMessageResponse()
        {
            Errors = new List<ApiError>();
        }

        public ApiResponse ToApiResponse() => new ApiResponse { Errors = Errors };

        public static async Task<ApiMessageResponse> FromMessage(HttpResponseMessage message)
        {
            if (message == null) return null;

            var response = new ApiMessageResponse
            {
                Message = message,
                ResponseBody = await message.Content.ReadAsStringAsync().ConfigureAwait(false)
            };

            if (!message.IsSuccessStatusCode)
            {
                response.HandleErrors();
            }

            return response;
        }

        protected void HandleErrors()
        {
            string errorMessage = !string.IsNullOrEmpty(ResponseBody) ? ResponseBody : Message.StatusCode.ToString();

            try
            {
                Errors = JsonHelper.Deserialize<List<ApiError>>(ResponseBody);

                if (!Errors.Any())
                {
                    Errors = Errors.Append(new ApiError { ErrorMessage = errorMessage });
                }
            }
            catch
            {
                Errors = Errors.Append(new ApiError { ErrorMessage = errorMessage });
            }
        }
    }
}
