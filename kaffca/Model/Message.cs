using System.Text.Json.Serialization;

namespace kaffca.Model
{
    public class MessageDTO
    {
        // --- Từ Transaction JSON ---
        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("sender_id")]
        public string SenderId { get; set; }

        [JsonPropertyName("receiver_id")]
        public string ReceiverId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("old_balance_sender")]
        public decimal OldBalanceSender { get; set; }

        [JsonPropertyName("new_balance_sender")]
        public decimal NewBalanceSender { get; set; }

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("ip_address")]
        public string IpAddress { get; set; }

        [JsonPropertyName("is_fraud")]
        public int IsFraud { get; set; } // Có thể chuyển thành bool nếu cần logic xử lý riêng

        [JsonPropertyName("fraud_type")]
        public string? FraudType { get; set; }

        // --- Từ Account Warning/Blacklist JSON ---
        // Lưu ý: sender_id hoặc receiver_id có thể map với account_id tùy logic của bạn
        [JsonPropertyName("account_id")]
        public string? AccountId { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("added_at")]
        public DateTime? AddedAt { get; set; }
    }
}
