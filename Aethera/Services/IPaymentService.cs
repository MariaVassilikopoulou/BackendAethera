namespace Aethera.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntentAsync(string orderId, decimal amount, string userEmail, string currency = "eur");
    }
}
