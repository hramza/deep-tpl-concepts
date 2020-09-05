using Hra.Framework.Sample.Models;
using Hra.Framework.Sample.Repositories;
using Hra.Framework.Utils;
using Hra.Framework.Web.Domain.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hra.Framework.Sample.Background
{
    public class CurrencyLimitsBackgroundService : BackgroundService
    {
        private readonly ICurrenyApiRepository _currenyApiRepository;
        private readonly ILogger<CurrencyLimitsBackgroundService> _logger;

        public CurrencyLimitsBackgroundService(ICurrenyApiRepository currenyApiRepository, ILogger<CurrencyLimitsBackgroundService> logger)
        {
            Ensure.NotNull(currenyApiRepository, nameof(currenyApiRepository));
            Ensure.NotNull(logger, nameof(logger));

            _currenyApiRepository = currenyApiRepository;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => HraTaskExtensions.RunOnBackgroundThread(
            GetCurrencyLimits,
            stoppingToken,
            (exception, message) => _logger.LogError(exception, message),
            TimeSpan.FromSeconds(10));

        private Task GetCurrencyLimits()
        {
            const string btc = "BTC";
            const string eur = "EUR";

            // for a large sets of data and fast parallelism, use the bellow code in order to split the enumerable to ordered partitions
            // for example
            #region ordered partitions
            /* await Enumerable.Range(1, 50)
                .Execute(5, _ =>
                {
                    var task = _currenyApiRepository.GetCurrenyLimitsAsync();
                    task.ContinueWith(apiResponse =>
                    {
                        ApiResponse<CurrencyResponse> response = apiResponse.Result;
                        if (response.Success
                        && response.Data.Response?.Pairs?.Any(_ => _.Symbol1.Equals(btc) && _.Symbol2.Equals(eur)) == true)
                        {
                            Pair first = response.Data.Response.Pairs.First(_ => _.Symbol1.Equals(btc) && _.Symbol2.Equals(eur));

                            _logger.LogInformation($"Bitcoin to Euro => Min price = {first.MinPrice}, Max price = {first.MaxPrice}");
                        }
                        else
                        {
                            _logger.LogError(string.Join(',', response.Errors.Select(error => error.ErrorMessage)));
                        }
                    });

                    return task;
                })
                .ConfigureAwait(false);
            */
            #endregion

            var task = _currenyApiRepository.GetCurrenyLimitsAsync();
            task.ContinueWith(apiResponse =>
            {
                ApiResponse<CurrencyResponse> response = apiResponse.Result;
                if (response.Success
                && response.Data.Response?.Pairs?.Any(_ => _.Symbol1.Equals(btc) && _.Symbol2.Equals(eur)) == true)
                {
                    Pair first = response.Data.Response.Pairs.First(_ => _.Symbol1.Equals(btc) && _.Symbol2.Equals(eur));

                    _logger.LogInformation($"Bitcoin to Euro => Min price = {first.MinPrice}, Max price = {first.MaxPrice}");
                }
                else
                {
                    _logger.LogError(string.Join(',', response.Errors.Select(error => error.ErrorMessage)));
                }
            });

            return task;
        }
    }
}
