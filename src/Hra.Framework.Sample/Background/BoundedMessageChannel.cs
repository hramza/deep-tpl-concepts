using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Hra.Framework.Sample.Background
{
    public class BoundedMessageChannel<T>
    {
        private const int MaxMessages = 200;

        private readonly Channel<T> _channel;

        private readonly ILogger<BoundedMessageChannel<T>> _logger;

        public BoundedMessageChannel(ILogger<BoundedMessageChannel<T>> logger)
        {
            _channel = Channel.CreateBounded<T>(new BoundedChannelOptions(MaxMessages)
            {
                SingleReader = true,
                SingleWriter = true
            });

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default) => _channel.Reader.ReadAllAsync(cancellationToken);

        public async Task<bool> WriteMessagesAsync(T item, CancellationToken cancellationToken = default)
        {
            while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                if (_channel.Writer.TryWrite(item))
                {
                    _logger.LogInformation($"A new message has been added to the channel {JsonSerializer.Serialize(item)}");

                    return true;
                }
            }

            return false;
        }

        public bool TryCompleteWriter(Exception ex = null) => _channel.Writer.TryComplete(ex);
    }
}
