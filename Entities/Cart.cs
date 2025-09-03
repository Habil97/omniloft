using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Verveo.Entities
{
    public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<CartItem> Items { get; set; } = new List<CartItem>();
}
}
