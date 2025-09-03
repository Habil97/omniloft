using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Verveo.DataAccess;
using Verveo.Entities;

namespace Verveo.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly SessionCartRepository _sessionCartRepository;
        private readonly IProductRepository _productRepository; // Product repository eklendi

        public CartService(ICartRepository cartRepository, IOrderRepository orderRepository, SessionCartRepository sessionCartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _sessionCartRepository = sessionCartRepository;
            _productRepository = productRepository; // Constructor'da atandı
        }

        public Cart GetCart(int userId)
        {
            return _cartRepository.GetByUserId(userId);
        }

        public Cart GetCartFromSession(ISession session)
        {
            return _sessionCartRepository.GetCartFromSession(session);
        }

        public void SaveCartToSession(ISession session, Cart cart)
        {
            _sessionCartRepository.SaveCartToSession(session, cart);
        }

        public void ClearCartFromSession(ISession session)
        {
            _sessionCartRepository.ClearCartFromSession(session);
        }

        public List<CartItem> GetCartItems(int userId)
        {
            var cart = _cartRepository.GetByUserId(userId);
            return cart?.Items ?? new List<CartItem>();
        }

        public void AddToCart(int userId, int productId, int quantity)
        {
            _cartRepository.AddItem(userId, productId, quantity);
        }

        // Oturum açmamış kullanıcı için session sepetine ekleme
        public void AddToCart(Cart cart, int productId, int quantity)
        {
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                cart.Items.Add(new CartItem { ProductId = productId, Quantity = quantity });
        }

        public void RemoveFromCart(int userId, int productId)
        {
            var cart = GetCart(userId);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Items.Remove(item);
                _cartRepository.Update(cart);
            }
        }

        public void ClearCart(int userId)
        {
            var cart = GetCart(userId);
            cart.Items.Clear();
            _cartRepository.Update(cart);
        }

        public void UpdateQuantity(int userId, int productId, int quantity)
        {
            var cart = _cartRepository.GetByUserId(userId);
            if (cart == null)
                throw new InvalidOperationException("Sepetiniz bulunmamaktadır.");

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem == null)
                throw new InvalidOperationException("Sepetinizde bu ürün bulunmamaktadır.");

            cartItem.Quantity = quantity;
            _cartRepository.Update(cart);
        }

        public void SaveCart(Cart cart)
        {
            _cartRepository.Update(cart);
        }

        public Order CreateOrderFromCart(int userId)
        {
            var cart = _cartRepository.GetByUserId(userId);
            if (cart == null || cart.Items == null || cart.Items.Count == 0)
                throw new InvalidOperationException("Sepetinizde ürün bulunmamaktadır.");

            // Sepet ürünlerinin Product nesnesi null ise doldur
            foreach (var item in cart.Items)
            {
                if (item.Product == null)
                {
                    // ProductRepository üzerinden ürünü çek
                    item.Product = _productRepository.GetById(item.ProductId);
                }
            }

            decimal totalPrice = cart.Items.Sum(i => (i.Product?.Price ?? 0) * i.Quantity);
            if (totalPrice <= 0)
                throw new InvalidOperationException("Sepet tutarı sıfır olamaz.");

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                Total = totalPrice,
                Status = OrderStatus.Beklemede,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Product?.Price ?? 0
                }).ToList()
            };

            _orderRepository.Add(order);
            _cartRepository.Clear(userId);
            return order;
        }

        // Sepet aktarımı: Kullanıcı login olduğunda session sepetini kullanıcıya aktar
        public void TransferSessionCartToUser(ISession session, int userId)
        {
            var sessionCart = GetCartFromSession(session);
            if (sessionCart != null && sessionCart.Items.Any())
            {
                foreach (var item in sessionCart.Items)
                {
                    AddToCart(userId, item.ProductId, item.Quantity);
                }
                ClearCartFromSession(session);
            }
        }
    }
}
