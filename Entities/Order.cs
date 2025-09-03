using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Verveo.Entities
{
    public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; } // veya TotalPrice
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
}
}
