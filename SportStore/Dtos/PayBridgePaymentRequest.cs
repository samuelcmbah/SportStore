namespace SportStore.Dtos
{
    public class PayBridgePaymentRequest
    {
        public string ExternalUserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public string ExternalReference { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        public string NotificationUrl { get; set; } = string.Empty;

    }


}
