using Confluent.Kafka;
using kaffca.Model;
using kaffca.Service;
using Microsoft.AspNetCore.Mvc;

namespace kaffca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly KafkaProducerService _kafka;
        private readonly ILogger<TestController> _logger;

        public TestController(KafkaProducerService kafka, ILogger<TestController> logger)
        {
            _kafka = kafka;
            _logger = logger;
        }
        
            
        [HttpPost]
        public async Task<IActionResult> Send([FromQuery] int count = 1, CancellationToken cancellationToken = default)
        {
            if (count <= 0) count = 1;
            if (count > 100) count = 100; // Cap to 100 for safety

            var messages = GenerateRandomMessages(count);
            var results = new List<object>();

            try
            {
                foreach (var messageData in messages)
                {
                    var result = await _kafka.SendAsync(messageData, cancellationToken);
                    results.Add(new
                    {
                        messageData.TransactionId,
                        messageData.Type,
                        messageData.Amount,
                        result.Topic,
                        Partition = result.Partition.Value,
                        Offset = result.Offset.Value
                    });
                }

                return Ok(new
                {
                    Message = $"Successfully sent {results.Count} messages to Kafka!",
                    Data = results
                });
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out while waiting for Kafka.");
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex, "Failed to send message to Kafka");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    Message = "Kafka unavailable",
                    ex.Error.Reason,
                    Code = ex.Error.Code.ToString(),
                    ex.Error.IsLocalError,
                    ex.Error.IsBrokerError,
                    ex.Error.IsFatal
                });
            }
        }

        private List<MessageDTO> GenerateRandomMessages(int count)
        {
            var random = new Random();
            var types = new[] { "PAYMENT", "TRANSFER", "WITHDRAW" };
            var locations = new[] { "Ho Chi Minh City, VN", "Ha Noi, VN", "Da Nang, VN", "Can Tho, VN" };
            var devices = new[] { "IPHONE_15_PRO_VNM", "SAMSUNG_S24_ULTRA", "XIAOMI_14_PRO", "WEB_CHROME_WINDOWS" };
            var reasons = new[] { "Suspected Money Laundering", "Unusual Activity", "Large Transaction", "New Device Login" };

            var messages = new List<MessageDTO>();

            for (int i = 0; i < count; i++)
            {
                var amount = (decimal)(random.Next(100, 100000) * 1000);
                var oldBalance = (decimal)(random.Next(5000, 100000) * 1000);
                var senderId = $"C{random.Next(100000000, 999999999)}";

                messages.Add(new MessageDTO
                {
                    TransactionId = $"TXN-{DateTime.Now:yyyyMMdd}-{random.Next(1000, 9999)}",
                    Timestamp = DateTime.UtcNow,
                    SenderId = senderId,
                    ReceiverId = $"M{random.Next(100000000, 999999999)}",
                    Type = types[random.Next(types.Length)],
                    Amount = amount,
                    OldBalanceSender = oldBalance,
                    NewBalanceSender = oldBalance - amount,
                    DeviceId = devices[random.Next(devices.Length)],
                    Location = locations[random.Next(locations.Length)],
                    IpAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                    IsFraud = random.Next(0, 10) == 0 ? 1 : 0, // 10% chance of being fraud
                    FraudType = null,

                    AccountId = senderId,
                    Reason = reasons[random.Next(reasons.Length)],
                    AddedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30))
                });
            }

            return messages;
        }
    }
}
