using Stripe;

namespace Aethera.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly string _apiKey;

        public StripePaymentService(IConfiguration config)
        {
            _apiKey = config["Stripe:SecretKey"] ?? throw new InvalidOperationException("Stripe:SecretKey is not configured.");
        }

        public async Task<string> CreatePaymentIntentAsync(string orderId, decimal amount, string userEmail, string currency = "eur")
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe expects cents
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card", "klarna", "amazon_pay" },
                ReceiptEmail = string.IsNullOrEmpty(userEmail) ? null : userEmail,
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", orderId },
                    { "userEmail", userEmail }
                }
            };

            var client = new StripeClient(_apiKey);
            var service = new PaymentIntentService(client);
            var intent = await service.CreateAsync(options);
            return intent.ClientSecret;
        }
    }
}
