using Confluent.Kafka;
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
        public async Task<IActionResult> Send(CancellationToken cancellationToken)
        {
            var data = new
            {
                Id = 1,
                Name = "Hello Kafka"
            };

            try
            {
                var result = await _kafka.SendAsync(data, cancellationToken);
                return Ok(new
                {
                    Message = "Sent to Kafka!",
                    result.Topic,
                    Partition = result.Partition.Value,
                    Offset = result.Offset.Value
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
    }
}
