using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new();

        public void Add(Order order) => _orders.Add(order);

        public Order GetById(int id) => _orders.FirstOrDefault(o => o.Id == id)!;

        public IEnumerable<Order> GetByUserId(int userId)
        {
            return _orders.Where(o => o.UserId == userId).ToList();
        }

        public IEnumerable<Order> GetAll() => _orders;

        public void Update(Order order)
        {
            var existing = GetById(order.Id);
            if (existing != null)
            {
                _orders.Remove(existing);
                _orders.Add(order);
            }
        }

        public void Delete(int id)
        {
            var order = GetById(id);
            if (order != null)
                _orders.Remove(order);
        }
    }
}
