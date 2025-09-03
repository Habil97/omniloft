using Verveo.Entities;
public interface IPaymentRepository
{
    void Add(Payment payment);
    Payment GetByOrderId(int orderId);
}
