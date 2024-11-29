using System.ComponentModel.DataAnnotations;

namespace PaystackIntegrationPracticeApi.Models
{
    //public class PaystackPOCOs
    //{
    //}

    public class PaystackInitializeTransactionRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public decimal Amount { get; set; }

    }

    public class PaystackInitializeTransactionResponse
    {
        public bool? Status { get; set; }
        public string? Message { get; set; }
        public PaystackAcceptPaymentResponseData? Data { get; set; }
    }

    public class PaystackAcceptPaymentResponseData
    {
        public string? authorization_url { get; set; }
        public string? access_code { get; set; }
        public string? reference { get; set; }
    }

    public class VerifyTransactionResponse
    {
        public bool? Status { get; set; }
        public string? Message { get; set; }
        public VerifyTransactionResponseData? Data { get; set; }
    }

    public class VerifyTransactionResponseData
    {
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
    }
}
