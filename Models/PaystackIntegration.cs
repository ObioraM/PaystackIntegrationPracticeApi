using System.ComponentModel.DataAnnotations;

namespace PaystackIntegrationPracticeApi.Models
{
    public class PaystackIntegration
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountInBaseDenomination { get; set; }
        public string? AccessCode { get; set; }
        public string? Reference { get; set; }
        public string? Status { get; set; }
        public string? IntializeTransactionResponseMessage { get; set; }
        public bool? TransactionVerified { get; set; }
        public string? TransactionVerificationMessage { get; set; }
        public string? VerifiedTransactionStatus { get; set; }
        public string? VerificationMeansUsed { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
