namespace kaffca.Model
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string SaslMechanism { get; set; } = "ScramSha256";
        public string? SslCaLocation { get; set; }
        public int MessageTimeoutMs { get; set; } = 10000;
        public int SocketTimeoutMs { get; set; } = 10000;
        public int RequestTimeoutMs { get; set; } = 5000;
    }
}
