using Verveo.Entities;
using System.Linq;
using Verveo.DataAccess;

public class EfPaymentRepository : IPaymentRepository
{
    private readonly VerveoDbContext _db;
    public EfPaymentRepository(VerveoDbContext db) { _db = db; }

    public void Add(Payment payment)
    {
        _db.Payments.Add(payment);
        _db.SaveChanges();
    }

    public Payment GetByOrderId(int orderId)
    {
        return _db.Payments.FirstOrDefault(p => p.OrderId == orderId)!;
    }
}