using StackExchange.Redis;

namespace kaffca.Service
{
    public class RedisService
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _redis;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = redis.GetDatabase();
        }

        public async Task SetAsync(string key, string value)
        {
            await _db.StringSetAsync(key, value);
        }

        // Hàm lấy TOÀN BỘ Key và Value dưới dạng Dictionary
        public async Task<Dictionary<string, string>> GetAllAsync()
        {
            var result = new Dictionary<string, string>();

            // 1. Lấy Endpoint và Server từ Multiplexer
            var endpoint = _redis.GetEndPoints()[0];
            var server = _redis.GetServer(endpoint);

            // 2. Duyệt qua tất cả các Key (Pattern "*" là lấy hết)
            // Phương thức này tự động dùng lệnh SCAN để tránh treo Redis
            foreach (var key in server.Keys(pattern: "*"))
            {
                // 3. Lấy giá trị tương ứng với Key
                var value = await _db.StringGetAsync(key);

                if (value.HasValue)
                {
                    result.Add(key.ToString(), value.ToString());
                }
            }

            return result;
        }
    }
}
