using Hra.Framework.Sample.Models;
using Hra.Framework.Web.Domain.Http;
using System.Threading.Tasks;

namespace Hra.Framework.Sample.Repositories
{
    public interface ICurrenyApiRepository
    {
        Task<ApiResponse<CurrencyResponse>> GetCurrenyLimitsAsync();

        Task<ResultItem> SearchCurrencyAsync(CurrencyRequest request);
    }
}
