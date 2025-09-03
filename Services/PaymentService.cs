using Verveo.Entities;
using Verveo.DataAccess;

public class PaymentService
{
    private readonly IPaymentRepository _paymentRepo;
    public PaymentService(IPaymentRepository paymentRepo) { _paymentRepo = paymentRepo; }

    public void ProcessPayment(int orderId, decimal amount)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            Amount = amount,
            Status = PaymentStatus.Basarili,
            PaidAt = DateTime.Now
        };
        _paymentRepo.Add(payment);
    }
}