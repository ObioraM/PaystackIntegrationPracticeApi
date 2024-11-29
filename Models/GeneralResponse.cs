namespace PaystackIntegrationPracticeApi.Models
{
    public class GeneralResponse
    {
        public string ResponseCode { get; set; }
        public bool IsSuccessful { get; set; }
        public string? Message { get; set; }
        public string? Data { get; set; }
    }
}
