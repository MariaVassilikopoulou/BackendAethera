using Aethera.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Aethera.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;

        public SendGridEmailService(IConfiguration config)
        {
            _apiKey = config["SendGrid:ApiKey"] ?? string.Empty;
            _fromEmail = config["SendGrid:FromEmail"] ?? string.Empty;
        }

        public async Task SendOrderConfirmationAsync(string toEmail, Order order)
        {
            var client = new SendGridClient(_apiKey);

            var itemRows = string.Join("", order.Items.Select(i =>
                $"<tr><td>{i.Name}</td><td>{i.Quantity}</td><td>€{i.Price:F2}</td><td>€{i.Price * i.Quantity:F2}</td></tr>"));

            var html = $"""
                <h2>Thank you for your order!</h2>
                <p>Order ID: <strong>{order.Id}</strong></p>
                <h3>Items</h3>
                <table border="1" cellpadding="6" style="border-collapse:collapse">
                  <thead>
                    <tr><th>Product</th><th>Qty</th><th>Unit Price</th><th>Subtotal</th></tr>
                  </thead>
                  <tbody>{itemRows}</tbody>
                </table>
                <p><strong>Total: €{order.TotalPrice:F2}</strong></p>
                <h3>Shipping to</h3>
                <p>
                  {order.ShippingAddress.FullName}<br/>
                  {order.ShippingAddress.Street}<br/>
                  {order.ShippingAddress.City}, {order.ShippingAddress.Postcode}<br/>
                  {order.ShippingAddress.Country}
                </p>
                """;

            var msg = MailHelper.CreateSingleEmail(
                from: new EmailAddress(_fromEmail, "Aethera"),
                to: new EmailAddress(toEmail),
                subject: $"Your Aethera Order #{order.Id[..8]}",
                plainTextContent: $"Order confirmed. Total: €{order.TotalPrice:F2}",
                htmlContent: html);

            await client.SendEmailAsync(msg);
        }
    }
}
