using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Verveo.DataAccess
{
    public class EfOrderRepository : IOrderRepository
{
    private readonly VerveoDbContext _db;
    public EfOrderRepository(VerveoDbContext db) { _db = db; }

    public void Add(Order order)
    {
        _db.Orders.Add(order);
        _db.SaveChanges();
    }

    public Order GetById(int id)
    {
        return _db.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(o => o.Id == id)!;
    }

    public IEnumerable<Order> GetByUserId(int userId)
    {
        return _db.Orders.Include(o => o.Items).ThenInclude(i => i.Product).Where(o => o.UserId == userId).ToList();
    }
public IEnumerable<Order> GetAll() => _db.Orders.Include(o => o.Items).ToList();
public void Update(Order order)
{
    _db.Orders.Update(order);
    _db.SaveChanges();
}
public void Delete(int id)
{
    var order = GetById(id);
    if (order != null)
    {
        _db.Orders.Remove(order);
        _db.SaveChanges();
    }
}
}
}
