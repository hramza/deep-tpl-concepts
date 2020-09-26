using Hra.Framework.Sample.Models;
using Hra.Framework.Sample.Repositories;
using Hra.Framework.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Hra.Framework.Sample.Background
{
    public class CurrencyLimitsBackgroundService : BackgroundService
    {
        private readonly ICurrenyApiRepository _currenyApiRepository;
        private readonly ILogger<CurrencyLimitsBackgroundService> _logger;
        private BoundedMessageChannel<CurrencyRequest> _boundedMessageChannel;

        public CurrencyLimitsBackgroundService(
            ICurrenyApiRepository currenyApiRepository,
            ILogger<CurrencyLimitsBackgroundService> logger,
            BoundedMessageChannel<CurrencyRequest> boundedMessageChannel)
        {
            Ensure.NotNull(currenyApiRepository, nameof(currenyApiRepository));
            Ensure.NotNull(logger, nameof(logger));
            Ensure.NotNull(boundedMessageChannel, nameof(boundedMessageChannel));

            _currenyApiRepository = currenyApiRepository;
            _logger = logger;
            _boundedMessageChannel = boundedMessageChannel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var request in _boundedMessageChannel.ReadAllAsync(stoppingToken))
            {
                var response = await _currenyApiRepository.SearchCurrencyAsync(request);

                if (!string.IsNullOrEmpty(response.Error))
                {
                    _logger.LogInformation($"An error occured while searching for data: {response.Error}");
                }
                else
                {
                    _logger.LogInformation($"{request.From} to {request.To} => Low price = {response.Low}, Max price = {response.High}");
                }
            }
        }
    }
}
