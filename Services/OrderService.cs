using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;
using Verveo.DataAccess;

namespace Verveo.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IEnumerable<Order> GetAll()
        {
            return _orderRepository.GetAll() ?? Enumerable.Empty<Order>();
        }

        public Order GetOrderById(int id) => _orderRepository.GetById(id);
        public void AddOrder(Order order) => _orderRepository.Add(order);
        public void UpdateOrder(Order order) => _orderRepository.Update(order);
        public void DeleteOrder(int id) => _orderRepository.Delete(id);

        // DÜZELTME: GetOrdersByUserId metodu GetAll ile çalışacak şekilde güncellendi
        public List<Order> GetOrdersByUserId(int userId)
        {
            var orders = _orderRepository.GetAll() ?? Enumerable.Empty<Order>();
            return orders.Where(o => o.UserId == userId).ToList();
        }

        public List<Order> GetAllOrders()
        {
            var orders = _orderRepository.GetAll() ?? Enumerable.Empty<Order>();
            return orders.ToList();
        }   
    }
}
