using Aethera.Dtos.Payment;
using Aethera.Interfaces;
using Aethera.Models;
using Aethera.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace Aethera.Controllers.Payment
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IGenericRepository<Aethera.Models.Order> _orderRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public PaymentController(
            IPaymentService paymentService,
            IGenericRepository<Aethera.Models.Order> orderRepository,
            IEmailService emailService,
            IConfiguration config)
        {
            _paymentService = paymentService;
            _orderRepository = orderRepository;
            _emailService = emailService;
            _config = config;
        }

        // POST /api/payment/intent — creates a Stripe PaymentIntent for an order
        [HttpPost("intent")]
        [Authorize]
        public async Task<IActionResult> CreateIntent([FromBody] CreatePaymentIntentDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new Exception("User ID not found in claims");
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

            var order = await _orderRepository.GetByIdAsync(dto.OrderId, userId);
            if (order == null || order.UserId != userId)
                return NotFound(new { message = "Order not found." });

            if (order.Status != OrderStatus.Pending)
                return BadRequest(new { message = "Payment can only be initiated for pending orders." });

            var clientSecret = await _paymentService.CreatePaymentIntentAsync(order.Id, order.TotalPrice, userEmail);

            // Store the PaymentIntent ID on the order so the webhook can look it up
            order.Status = OrderStatus.AwaitingPayment;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order, userId);

            return Ok(new PaymentIntentResponseDto { ClientSecret = clientSecret });
        }

        // POST /api/payment/webhook — called by Stripe after payment is confirmed
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var webhookSecret = _config["Stripe:WebhookSecret"];

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret);
            }
            catch (StripeException)
            {
                return BadRequest();
            }

            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent?.Metadata.TryGetValue("orderId", out var orderId) == true)
                {
                    var orders = await _orderRepository.FindAsync(o => o.Id == orderId);
                    var order = orders.FirstOrDefault();
                    if (order != null)
                    {
                        order.Status = OrderStatus.Paid;
                        order.StripePaymentIntentId = paymentIntent.Id;
                        order.UpdatedAt = DateTime.UtcNow;
                        await _orderRepository.UpdateAsync(order, order.UserId);

                        var userEmail = paymentIntent.ReceiptEmail
                            ?? paymentIntent.Metadata.GetValueOrDefault("userEmail");

                        if (!string.IsNullOrEmpty(userEmail))
                            await _emailService.SendOrderConfirmationAsync(userEmail, order);
                    }
                }
            }

            return Ok();
        }
    }
}
