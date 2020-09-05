using Hra.Framework.Web.Domain.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Hra.Framework.Web.Implementations
{
    public class ApiClientBase
    {
        private readonly Func<bool, Task<string>> _getAccessToken;
        private readonly HttpClient _httpClient;

        public ApiClientBase(HttpClient httpclient, Func<bool, Task<string>> getAccessToken)
        => (_httpClient, _getAccessToken) = (httpclient, getAccessToken);

        public ApiClientBase(HttpClient httpClient) => _httpClient = httpClient;

        private string BaseAddress => _httpClient.BaseAddress.ToString();

        protected async Task<ApiMessageResponse<T>> GetHttp<T>(string path)
        => await SendHttp<T>(() => new HttpRequestMessage(HttpMethod.Get, new Uri(BaseAddress + path))).ConfigureAwait(false);

        protected async Task<ApiMessageResponse<T>> PostHttp<T>(string path, object @object, bool withCompleteUrl)
        => await SendHttp<T>(() => new HttpRequestMessage(HttpMethod.Post, withCompleteUrl ? new Uri(path) : new Uri(BaseAddress + path))
        {
            Content = GetJsonBody(@object)
        }).ConfigureAwait(false);

        private async Task<ApiMessageResponse<T>> SendHttp<T>(Func<HttpRequestMessage> requestMessageFunc)
        {
            try
            {
                var requestMessage = requestMessageFunc();

                if (_getAccessToken != null) await SetAuthHeader(requestMessage, false).ConfigureAwait(false);

                var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                if (_getAccessToken != null && response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    requestMessage = requestMessageFunc();

                    await SetAuthHeader(requestMessage, true).ConfigureAwait(false);

                    response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                }

                return await ApiMessageResponse<T>.FromMessage(response).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new ApiMessageResponse<T>
                {
                    Errors = new List<ApiError> { new ApiError { ErrorMessage = GetErrorMessage(ex) } }
                };
            }
        }

        private string GetErrorMessage(Exception ex, string delimeter = " --- ")
        {
            Exception current = ex;
            StringBuilder builder = new StringBuilder();

            while (current != null)
            {
                if (!string.IsNullOrEmpty(builder.ToString())) builder.Append(delimeter);

                builder.Append(current.Message);

                current = current.InnerException;
            }

            return builder.ToString();
        }

        private async Task SetAuthHeader(HttpRequestMessage requestMessage, bool renew)
        {
            var token = await _getAccessToken(renew).ConfigureAwait(false);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        private HttpContent GetPostBody(Dictionary<string, string> parameters)
        => new StringContent(string.Join("&", parameters.Select(x => $"{x.Key}={x.Value}")), Encoding.UTF8, "application/x-www-form-urlencoded");

        private HttpContent GetJsonBody(object @object)
        => new StringContent(JsonHelper.Serialize(@object), Encoding.UTF8, "application/json");
    }
}
