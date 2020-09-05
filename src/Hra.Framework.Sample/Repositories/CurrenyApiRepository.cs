using Hra.Framework.Sample.Models;
using Hra.Framework.Web.Domain.Http;
using Hra.Framework.Web.Implementations;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hra.Framework.Sample.Repositories
{
    public class CurrenyApiRepository : ApiClientBase, ICurrenyApiRepository
    {
        public CurrenyApiRepository(HttpClient httpClient) : base(httpClient) { }

        public async Task<ApiResponse<CurrencyResponse>> GetCurrenyLimitsAsync()
        {
            var response = await GetHttp<CurrencyResponse>("api/currency_limits").ConfigureAwait(false);

            return response.ToApiResponse();
        }
    }
}
