namespace Aethera.Dtos.Payment
{
    public class CreatePaymentIntentDto
    {
        public string OrderId { get; set; } = string.Empty;
    }

    public class PaymentIntentResponseDto
    {
        public string ClientSecret { get; set; } = string.Empty;
    }
}
