using kaffca.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kaffca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestRedisController : ControllerBase
    {
        private readonly RedisService _redis;

        public TestRedisController(RedisService redis)
        {
            _redis = redis;
        }

        [HttpPost("set")]
        public async Task<IActionResult> Set(string key, string value)
        {
            await _redis.SetAsync(key, value);
            return Ok("Saved!");
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(string key)
        {
            var value = await _redis.GetAsync(key);
            return Ok(value);
        }
    }
}
