namespace kaffca.Model
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Topic { get; set; }
        public string GroupId { get; set; }
    }
}
