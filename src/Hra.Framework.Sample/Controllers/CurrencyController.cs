using Hra.Framework.Sample.Background;
using Hra.Framework.Sample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Hra.Framework.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly BoundedMessageChannel<CurrencyRequest> _boundedMessageChannel;

        public CurrencyController(BoundedMessageChannel<CurrencyRequest> boundedMessageChannel)
        => _boundedMessageChannel = boundedMessageChannel;

        /// <summary>
        /// from "BTC" to "EUR"
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] CurrencyRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrEmpty(request.From) || string.IsNullOrEmpty(request.To)) return BadRequest();

            await _boundedMessageChannel.WriteMessagesAsync(request, cancellationToken);

            return Accepted();
        }
    }
}
