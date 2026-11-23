namespace basic_ecommerce_platform.Payment
{
    public record PaymentResult(
        bool Success,
        string? FailureReason
        );

    public interface IPaymentGateway
    {
        Task<PaymentResult> ChargeAsync(decimal amount, string token);
    }
    public class MockPaymentGateway : IPaymentGateway
    {
        public async Task<PaymentResult> ChargeAsync(decimal amount, string token)
        {
            await Task.Delay(300); // Simulate network delay

            return token switch
            {
                "tok_visa"              => new PaymentResult(true, null),
                "tok_chargeDeclined"    => new PaymentResult(false, "Card declined."),
                "tok_insufficientFunds" => new PaymentResult(false, "Insufficient funds."),
                _                       => new PaymentResult(false, "Invalid payment token.")
            };
        }
    }
}
