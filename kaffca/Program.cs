
using kaffca.Model;
using kaffca.Service;
using StackExchange.Redis;

namespace kaffca
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.Configure<KafkaSettings>(
              builder.Configuration.GetSection("Kafka"));

            builder.Services.AddSingleton<KafkaProducerService>();

            // l?y connection string
            var redisConnection = builder.Configuration.GetSection("Redis")["ConnectionString"];

            // ??ng ký Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnection)
            );

            builder.Services.AddScoped<RedisService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
