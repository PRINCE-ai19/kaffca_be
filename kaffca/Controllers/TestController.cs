using kaffca.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kaffca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly KafkaProducerService _kafka;
        public TestController(KafkaProducerService kafka) // Dependency Injection of KafkaProducerService
        {
            _kafka = kafka;
        }

        [HttpPost]
        public async Task<IActionResult> Send()
        {
            var data = new
            {
                Id = 1,
                Name = "Hello Kafka"
            };

            await _kafka.SendAsync(data);

            return Ok("Sent to Kafka!");
        }
    }
}
