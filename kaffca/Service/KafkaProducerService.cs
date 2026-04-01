using Confluent.Kafka;
using kaffca.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace kaffca.Service
{
    public class KafkaProducerService : IDisposable
    {
        private readonly KafkaSettings _settings;
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly IWebHostEnvironment _environment;

        public KafkaProducerService(
            IOptions<KafkaSettings> options,
            ILogger<KafkaProducerService> logger,
            IWebHostEnvironment environment)
        {
            _settings = options.Value;
            _logger = logger;
            _environment = environment;
            var mechanism = Enum.TryParse<SaslMechanism>(_settings.SaslMechanism, true, out var parsedMechanism)
                ? parsedMechanism
                : SaslMechanism.ScramSha256;

            var config = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = mechanism,
                SaslUsername = _settings.Username,
                SaslPassword = _settings.Password,
                MessageTimeoutMs = _settings.MessageTimeoutMs,
                SocketTimeoutMs = _settings.SocketTimeoutMs,
                RequestTimeoutMs = _settings.RequestTimeoutMs
            };

            var caLocation = _settings.SslCaLocation;
            if (string.IsNullOrWhiteSpace(caLocation))
            {
                var defaultCaPath = Path.Combine(_environment.ContentRootPath, "ca.pem");
                if (File.Exists(defaultCaPath))
                {
                    caLocation = defaultCaPath;
                }
            }

            if (!string.IsNullOrWhiteSpace(caLocation))
            {
                config.SslCaLocation = caLocation;
            }

            _logger.LogInformation(
                "Kafka producer config loaded. BootstrapServers={BootstrapServers}, Topic={Topic}, SaslMechanism={SaslMechanism}, SslCaLocation={SslCaLocation}",
                _settings.BootstrapServers,
                _settings.Topic,
                mechanism,
                string.IsNullOrWhiteSpace(caLocation) ? "(none)" : caLocation);

            _producer = new ProducerBuilder<Null, string>(config)
                .SetErrorHandler((_, error) =>
                {
                    _logger.LogError(
                        "Kafka producer error: Code={Code}, Reason={Reason}, IsLocal={IsLocal}, IsBroker={IsBroker}, IsFatal={IsFatal}",
                        error.Code,
                        error.Reason,
                        error.IsLocalError,
                        error.IsBrokerError,
                        error.IsFatal);
                })
                .Build();
        }

        public async Task<DeliveryResult<Null, string>> SendAsync(object data, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(data);
            return await _producer.ProduceAsync(
                _settings.Topic,
                new Message<Null, string> { Value = json },
                cancellationToken);
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(2));
            _producer.Dispose();
        }
    }
}
