using PaystackIntegrationPracticeApi.Models;

namespace PaystackIntegrationPracticeApi.Interfaces
{
    public interface IPaystackIntegrationService
    {
        Task<GeneralResponse> InitializeTransaction(PaystackInitializeTransactionRequest request);
        Task<IEnumerable<PaystackIntegration>> GetAllTransactions();
        Task<GeneralResponse> VerifyTransaction(string? request);
        Task<bool> Webhook(string? request, string xpaystacksignature);



        //
        Task AddRecordForTest();
    }
}
