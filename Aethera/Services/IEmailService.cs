using Aethera.Models;

namespace Aethera.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(string toEmail, Order order);
    }
}
