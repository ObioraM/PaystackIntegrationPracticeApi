using Microsoft.AspNetCore.Mvc;
using PaystackIntegrationPracticeApi.ActionFilter;
using PaystackIntegrationPracticeApi.Interfaces;
using PaystackIntegrationPracticeApi.Models;

namespace PaystackIntegrationPracticeApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaystackIntegrationController : ControllerBase
    {
        private readonly ILogger<PaystackIntegrationController> _logger;
        private readonly IPaystackIntegrationService _paystackIntegrationService;

        public PaystackIntegrationController(ILogger<PaystackIntegrationController> logger,
            IPaystackIntegrationService paystackIntegrationService)
        {
            _logger = logger;
            _paystackIntegrationService = paystackIntegrationService;
        }

        [HttpGet]
        public IActionResult GetAllRecords()
        {
            var paystackTransactionRecords = _paystackIntegrationService.GetAllTransactions();
            return Ok(paystackTransactionRecords);
        }

        [HttpPost("initializetransaction")]
        public async Task<IActionResult> InitializeTransaction(PaystackInitializeTransactionRequest request)
        {
            // Todo: Validate request object

            var response = await _paystackIntegrationService.InitializeTransaction(request);
            return Ok(response);
        }

        // https://paystack.com/docs/payments/verify-payments/
        [HttpGet("verifytransaction/{reference}")]
        public async Task<IActionResult> VerifyTransaction(string? reference)
        {
            // Todo: Validate request object

            var response = await _paystackIntegrationService.VerifyTransaction(reference);
            return Ok(response);
        }

        [ServiceFilter(typeof(PaystackWebhookIPFilter))]
        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            // Todo: Validate request object

            // Get the x-paystack-signature from the headers
            if (!Request.Headers.TryGetValue("x-paystack-signature", out var xpaystackSignature))
            {
                return BadRequest("Missing x-paystack-signature header");
            }

            string jsonInput = string.Empty;
            using (var reader = new StreamReader(Request.Body))
            {
                jsonInput = await reader.ReadToEndAsync();
            }
            bool eventHandled = await _paystackIntegrationService.Webhook(jsonInput, xpaystackSignature.ToString());
            if (eventHandled)
            {
                return Ok();
            }

            return BadRequest();
        }


        [HttpGet("addrecordfortest")]
        public async Task<IActionResult> AddRecordForTest()
        {
            // Todo: Validate request object

            await _paystackIntegrationService.AddRecordForTest();
            return Ok();
        }


    }
}
