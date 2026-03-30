using Confluent.Kafka;
using kaffca.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace kaffca.Service
{
    public class KafkaProducerService
    {
        private readonly KafkaSettings _settings;

        public KafkaProducerService(IOptions<KafkaSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(object data)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers,

                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,

                SaslUsername = _settings.Username,
                SaslPassword = _settings.Password
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var json = JsonSerializer.Serialize(data);

            await producer.ProduceAsync(_settings.Topic, new Message<Null, string>
            {
                Value = json
            });
        }
    }
}
