using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public class EfReturnRequestRepository : IReturnRequestRepository
{
    private readonly VerveoDbContext _db;
    public EfReturnRequestRepository(VerveoDbContext db) { _db = db; }

    public void Add(ReturnRequest request)
    {
        _db.ReturnRequests.Add(request);
        _db.SaveChanges();
    }

    public IEnumerable<ReturnRequest> GetByUserId(int userId)
    {
        return _db.ReturnRequests.Where(r => r.UserId == userId).ToList();
    }

    public IEnumerable<ReturnRequest> GetAll()
    {
        return _db.ReturnRequests.ToList();
    }

    public void Update(ReturnRequest request)
    {
        _db.ReturnRequests.Update(request);
        _db.SaveChanges();
    }

    public ReturnRequest GetById(int id) => _db.ReturnRequests.Find(id);

    public void Delete(int id)
    {
        var req = GetById(id);
        if (req != null)
        {
            _db.ReturnRequests.Remove(req);
            _db.SaveChanges();
        }
    }

    public IEnumerable<ReturnRequest> GetByOrderId(int orderId)
    {
        return _db.ReturnRequests.Where(r => r.OrderId == orderId).ToList();
    }
}
}