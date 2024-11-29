using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PaystackIntegrationPracticeApi.Data;
using PaystackIntegrationPracticeApi.Interfaces;
using PaystackIntegrationPracticeApi.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PaystackIntegrationPracticeApi.Services
{
    public class PaystackIntegrationService : IPaystackIntegrationService
    {
        private readonly ILogger<PaystackIntegrationService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _dbContext;

        public PaystackIntegrationService(ILogger<PaystackIntegrationService> logger,
            IHttpClientFactory httpClientFactory, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
        }


        public async Task AddRecordForTest()
        {
            PaystackIntegration paystackIntegration = new();
            paystackIntegration.Email = "an_email";
            paystackIntegration.Amount = 1000;
            paystackIntegration.Reference = Guid.NewGuid().ToString();
            paystackIntegration.CreatedDate = DateTime.UtcNow;

            _dbContext.PaystackIntegration.Add(paystackIntegration);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GeneralResponse> InitializeTransaction(PaystackInitializeTransactionRequest request)
        {
            // paystack test secret key: sk_test_3442fefa9693af3d9955a0edcb33f034bd1b9cf0
            // paystack test public key: pk_test_1c56cb1c1defd0df15a1969df89e8cfcab557266

            var httpClient = _httpClientFactory.CreateClient("paystack");
            //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "sk_test_3442fefa9693af3d9955a0edcb33f034bd1b9cf0");

            decimal amountInBaseDenomination = request.Amount * 100;

            PaystackIntegration paystackIntegration = new();

            // About to Initialize transaction
            paystackIntegration.Email = request.Email;
            paystackIntegration.Amount = request.Amount;
            paystackIntegration.AmountInBaseDenomination = amountInBaseDenomination;
            paystackIntegration.Reference = Guid.NewGuid().ToString();
            paystackIntegration.Status = "AboutToSendTransactionInitializeRequest";
            paystackIntegration.CreatedDate = DateTime.UtcNow;

            // Add Record to table
            try
            {
                _dbContext.PaystackIntegration.Add(paystackIntegration);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

                return new GeneralResponse
                {
                    IsSuccessful = false,
                    Message = "Something went wrong. Please try again later",
                    ResponseCode = "99"
                };
            }

            // Form data is typically sent as key-value pairs
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("email", request.Email),
                new KeyValuePair<string, string>("amount", amountInBaseDenomination.ToString()),
                new KeyValuePair<string, string>("reference", paystackIntegration.Reference)
            };

            // Encodes the key-value pairs for the ContentType 'application/x-www-form-urlencoded'
            HttpContent content = new FormUrlEncodedContent(formData);

            try
            {
                // Send a POST request to the specified Uri as an asynchronous operation.
                //using HttpResponseMessage response = await httpClient.PostAsync("https://api.paystack.co/transaction/initialize", content);
                using HttpResponseMessage response = await httpClient.PostAsync("transaction/initialize", content);

                string responseData = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("HTTP Status Code {statusCode}. Content {responseContent}", response.StatusCode, responseData);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) 
                {
                    var paymentRequestUnauthorizedResponse = JsonSerializer.Deserialize<PaystackInitializeTransactionResponse>(responseData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });

                    paystackIntegration.UpdatedDate = DateTime.UtcNow;
                    if (paymentRequestUnauthorizedResponse?.Message is not null) paystackIntegration.IntializeTransactionResponseMessage = paymentRequestUnauthorizedResponse.Message;
                    if (paymentRequestUnauthorizedResponse?.Status is false) paystackIntegration.Status = "TransactionInitializeAttemptFailed";

                    await _dbContext.SaveChangesAsync();

                    return new GeneralResponse
                    {
                        IsSuccessful = false,
                        Message = "Something went wrong. Please try again later",
                        ResponseCode = "99"
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    paystackIntegration.UpdatedDate = DateTime.UtcNow;
                    paystackIntegration.Status = "InternalServerErrorOnThirdParty";

                    await _dbContext.SaveChangesAsync();

                    return new GeneralResponse
                    {
                        IsSuccessful = false,
                        Message = "Something went wrong. Please try again later",
                        ResponseCode = "99"
                    };
                }
                else if (!response.IsSuccessStatusCode)
                {
                    paystackIntegration.UpdatedDate = DateTime.UtcNow;
                    paystackIntegration.Status = $"ErrorHttpStatusCode {response.StatusCode}";

                    await _dbContext.SaveChangesAsync();

                    return new GeneralResponse
                    {
                        IsSuccessful = false,
                        Message = "Something went wrong. Please try again later",
                        ResponseCode = "99"
                    };
                }

                var paymentRequestResponse = JsonSerializer.Deserialize<PaystackInitializeTransactionResponse>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                paystackIntegration.UpdatedDate = DateTime.UtcNow;
                if (paymentRequestResponse?.Data?.access_code is not null) paystackIntegration.AccessCode = paymentRequestResponse?.Data?.access_code;
                if (paymentRequestResponse?.Message is not null) paystackIntegration.IntializeTransactionResponseMessage = paymentRequestResponse.Message;
                if (paymentRequestResponse?.Status is true) paystackIntegration.Status = "TransactionInitializeAttemptSuccessful";
                if (paymentRequestResponse?.Status is false) paystackIntegration.Status = "TransactionInitializeAttemptFailed";

                await _dbContext.SaveChangesAsync();

                Console.WriteLine(responseData);

                return new GeneralResponse { 
                    IsSuccessful = true, 
                    Message = paymentRequestResponse?.Message, 
                    ResponseCode = "00", 
                    Data = paymentRequestResponse?.Data?.authorization_url 
                };
            }
            catch (HttpRequestException e)
            {
                //Console.WriteLine("Error: " + e.Message);
                _logger.LogError(e, "Error: " + e.Message);
                return new GeneralResponse
                {
                    IsSuccessful = false,
                    Message = "Something went wrong. Please try again later",
                    ResponseCode = "99"
                };
            }
        }

        // https://paystack.com/docs/payments/verify-payments/
        public async Task<GeneralResponse> VerifyTransaction(string? reference)
        {
            // Validate input
                // If not valid return informing of invalid input

            // Get corresponding paystack record from db
                // If not there return that transaction with reference not found in db
            var paystackRecord = await _dbContext.PaystackIntegration.FirstOrDefaultAsync(x => x.Reference == reference);
            
            if (paystackRecord is null)
            {
                _logger.LogInformation("No paystack record found on db with reference {referenceNotFoundOnDb}", reference);
                return new GeneralResponse 
                { 
                    IsSuccessful = false, 
                    ResponseCode = "99", 
                    Message = $"Record with reference {reference} not found. Confirm that reference is correct and try again." 
                }; 
            }

            _logger.LogInformation("Paystack record on db with reference {paystackRecordReference} : {paystackRecord}", 
                reference, JsonSerializer.Serialize(paystackRecord));

            // To do: Check if transaction is already verified and updated by paystack call the webhook url.
            // At this point, I see that this would only be sufficient if the record shows that the transaction has been completed

            // Call paystack for transaction with reference
            // Update record accordingly and save to db

            var httpClient = _httpClientFactory.CreateClient("paystack");
            using HttpResponseMessage response = await httpClient.GetAsync($"transaction/verify/{reference}");

            _logger.LogInformation("HttpStatusCode of response is {HttpStatusCode}", response.StatusCode);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                _logger.LogInformation("Could not verify transaction.");

                return new GeneralResponse
                {
                    IsSuccessful = false,
                    ResponseCode = "99",
                    Message = $"Thirdparty service unavailable."
                };
            }

            var responseData = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Response content: " +  responseData);

            var verifyTransactionResponse = JsonSerializer.Deserialize<VerifyTransactionResponse>(responseData, 
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

            if (verifyTransactionResponse is null)
            {
                _logger.LogError("Unable to deserialize verify transaction response");
                return new GeneralResponse 
                { 
                    IsSuccessful = false,
                    ResponseCode = "99",
                    Message = "An error occured. Please try again later."
                };
            }

            if (verifyTransactionResponse.Status is null )
            {
                _logger.LogError("Unable to verify transaction: status field is not set.");
                return new GeneralResponse
                {
                    IsSuccessful = false,
                    ResponseCode = "99",
                    Message = "An error occured. Please try again later."
                };
            }

            if (verifyTransactionResponse.Message is null)
            {
                _logger.LogError("Unable to verify transaction: message field is not set.");
                return new GeneralResponse
                {
                    IsSuccessful = false,
                    ResponseCode = "99",
                    Message = "An error occured. Please try again later."
                };
            }

            // To do: Paystack advises that the amount be also confirmed. I believe
            // this would require the email and amount to also be passed in the request
            // and those compared again what we have on the db, if any does not match
            // return an error message immediately... currently only taking reference


            paystackRecord.TransactionVerified = verifyTransactionResponse.Status;
            paystackRecord.TransactionVerificationMessage = verifyTransactionResponse.Message;
            paystackRecord.VerificationMeansUsed = "VerifyApi";
            paystackRecord.UpdatedDate = DateTime.UtcNow;

            if (verifyTransactionResponse.Data is null || 
                verifyTransactionResponse.Data.Status is null)
            {
                _logger.LogInformation("Response does not have data field");
                await _dbContext.SaveChangesAsync();

                return new GeneralResponse
                {
                    IsSuccessful = false,
                    ResponseCode = "99",
                    Message = verifyTransactionResponse.Message + " Unable to verify exact state of transaction"
                };
            }

            paystackRecord.VerifiedTransactionStatus = verifyTransactionResponse.Data.Status;
            await _dbContext.SaveChangesAsync();

            if (verifyTransactionResponse.Data.Status.ToLower().Equals("success"))
            {
                _logger.LogInformation("Returning success response for transaction verification");

                return new GeneralResponse
                {
                    IsSuccessful = true,
                    ResponseCode = "00",
                    Message = verifyTransactionResponse.Message + ". Status: " + verifyTransactionResponse.Data.Status
                };
            }

            _logger.LogInformation("Returning success response for transaction verification");

            return new GeneralResponse
            {
                IsSuccessful = false,
                ResponseCode = "99",
                Message = verifyTransactionResponse.Message + ". Status: " + verifyTransactionResponse.Data.Status
            };

            // I don't think it is necessary to check the status code again, 
            // since badrequests also return with payload with useful info.
            // Also, we may get a 200 ok response even when the transaction
            // has not been completed. Internal server error which does not
            // return any content has been handled explicity above.
            //if (!response.IsSuccessStatusCode)
            //{

            //}
            //else if (response.IsSuccessStatusCode)
            //{

            //}
        }

        public async Task<IEnumerable<PaystackIntegration>> GetAllTransactions()
        {
            return await _dbContext.PaystackIntegration.ToListAsync();
        }

        public async Task<bool> Webhook(string? jsonInput, string xpaystacksignature)
        {
            _logger.LogInformation("{jsonInput} {xpaystacksignature}", jsonInput, xpaystacksignature);

            // paystack test secret key: sk_test_3442fefa9693af3d9955a0edcb33f034bd1b9cf0
            // paystack test public key: pk_test_1c56cb1c1defd0df15a1969df89e8cfcab557266
            string key = "sk_test_3442fefa9693af3d9955a0edcb33f034bd1b9cf0";

            if (jsonInput is null)
            {
                return false;
            }

            string? inputString = Convert.ToString(new JValue(jsonInput));

            if (inputString is null)
            {
                return false;
            }

            string result = string.Empty;
            byte[] secretkeyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);

            using (var hmac = new HMACSHA512(secretkeyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                result = BitConverter.ToString(hashValue).Replace("-", string.Empty);
            }

            _logger.LogInformation(result);

            if (!result.ToLower().Equals(xpaystacksignature))
            {
                return false;
            }

            var paystackEvent = JsonSerializer.Deserialize<TransactionSuccessfulEvent>(jsonInput, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (paystackEvent is null ||
                paystackEvent.Data is null ||
                paystackEvent.Event is null ||
                paystackEvent.Data.Reference is null ||
                paystackEvent.Data.Amount is null ||
                paystackEvent.Data.Status is null)
            {
                return false;
            }
            
            if (!paystackEvent.Event.Equals("charge.success"))
            {
                return false;
            }

            var paystackRecord = await _dbContext.PaystackIntegration.FirstOrDefaultAsync(x => x.Reference == paystackEvent.Data.Reference);

            if (paystackRecord is null)
            {
                _logger.LogInformation("No paystack record found on db with reference {referenceNotFoundOnDb}", paystackEvent.Data.Reference);
                return false;
            }

            _logger.LogInformation("Paystack record on db with reference {paystackRecordReference} : {paystackRecord}",
                paystackEvent.Data.Reference, JsonSerializer.Serialize(paystackRecord));

            if (paystackRecord.AmountInBaseDenomination != paystackEvent.Data.Amount)
            {
                return false;
            }

            paystackRecord.TransactionVerified = true;
            paystackRecord.VerifiedTransactionStatus = paystackEvent.Data.Status;
            if (paystackEvent.Data.Message is not null) paystackRecord.TransactionVerificationMessage = paystackEvent.Data.Message;
            paystackRecord.VerificationMeansUsed = "WebhookUrl";
            paystackRecord.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
