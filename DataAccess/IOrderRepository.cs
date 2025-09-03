using System.Collections.Generic;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public interface IOrderRepository
{
    void Add(Order order);
    Order GetById(int id);
    IEnumerable<Order> GetByUserId(int userId);
    IEnumerable<Order> GetAll();
    void Update(Order order);
    void Delete(int id);
}
}
