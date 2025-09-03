using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public class InMemoryCartRepository : ICartRepository
    {
        private readonly List<Cart> _carts = new();

        public Cart GetByUserId(int userId)
        {
            return _carts.FirstOrDefault(c => c.UserId == userId);
        }

        public void AddItem(int userId, int productId, int quantity)
        {
            var cart = GetByUserId(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _carts.Add(cart);
            }
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
                item.Quantity += quantity;
            else
                cart.Items.Add(new CartItem { ProductId = productId, Quantity = quantity });
        }

        public void RemoveItem(int userId, int productId)
        {
            var cart = GetByUserId(userId);
            if (cart == null) return;
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null) cart.Items.Remove(item);
        }

        public void Clear(int userId)
        {
            var cart = GetByUserId(userId);
            if (cart == null) return;
            cart.Items.Clear();
        }

        public void Update(Cart cart)
        {
            var existing = _carts.FirstOrDefault(c => c.Id == cart.Id);
            if (existing != null)
            {
                _carts.Remove(existing);
                _carts.Add(cart);
            }
        }
    }
}
