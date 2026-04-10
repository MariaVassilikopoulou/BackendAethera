using Stripe;

namespace Aethera.Services
{
    public class StripePaymentService : IPaymentService
    {
        public StripePaymentService(IConfiguration config)
        {
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
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

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);
            return intent.ClientSecret;
        }
    }
}
