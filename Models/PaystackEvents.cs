namespace PaystackIntegrationPracticeApi.Models
{
    //public class PaystackEvents
    //{
    //}

    // https://paystack.com/docs/payments/webhooks/#supported-events

    public class TransactionSuccessfulEvent
    {
        public string? Event { get; set; }
        public TransactionSuccessfulEventData? Data { get; set; }
    }

    public class TransactionSuccessfulEventData
    {
        public long? Id { get; set; }
        public string? Domain { get; set; }
        public string? Status { get; set; }
        public string? Reference { get; set; }
        public decimal? Amount { get; set; }
        public string? Message { get; set; }
        public string? Gateway_response { get; set; }
        public string? Paid_at { get; set; }
        public string? Created_at { get; set; }
        public string? Channel { get; set; }
        public string? Currency { get; set; }
        public string? Ip_address { get; set; }

    }

    public class PaymentRequestPendingEvent
    {
        public string Event { get; set; }
        public int Data { get; set; }
    }

    public class PaymentRequestPendingEventData
    {
        public string? Id { get; set; }
        public string? Domain { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? Request_code { get; set; }

    }


    public class PaymentRequestSuccessfulEvent
    {
        public string Event { get; set; }
        public PaymentRequestSuccessfulEventData Data { get; set; }
    }

    public class PaymentRequestSuccessfulEventData
    {
        public string? Id { get; set; }
        public string? Domain { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? Request_code { get; set; }
    }
}
