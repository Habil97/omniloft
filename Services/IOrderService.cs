using System.Collections.Generic;
using Verveo.Entities;

public interface IOrderService
{
    List<Order> GetOrdersByUserId(int userId);
    Order GetOrderById(int orderId);
    void AddOrder(Order order);
    List<Order> GetAllOrders();
}