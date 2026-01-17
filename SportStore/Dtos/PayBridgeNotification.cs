namespace SportStore.Dtos
{
    public class PayBridgeNotification
    {
        public string PaymentReference { get; set; } = string.Empty;
        public string ExternalReference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
