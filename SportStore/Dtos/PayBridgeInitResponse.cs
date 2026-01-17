namespace SportStore.Dtos
{
    public class PayBridgeInitResponse
    {
        public string AuthorizationUrl { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }
}
