using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;
using System;
using Microsoft.EntityFrameworkCore;

namespace Verveo.DataAccess
{
    public class EfCartRepository : ICartRepository
{
    private readonly VerveoDbContext _db;
    public EfCartRepository(VerveoDbContext db) { _db = db; }

    public Cart GetByUserId(int userId)
    {
        return _db.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(c => c.UserId == userId)!;
    }

    public void AddItem(int userId, int productId, int quantity)
    {
        var cart = GetByUserId(userId) ?? new Cart { UserId = userId };
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
            item.Quantity += quantity;
        else
            cart.Items.Add(new CartItem { ProductId = productId, Quantity = quantity, Product = _db.Products.Find(productId) });
        if (cart.Id == 0) _db.Carts.Add(cart);
        _db.SaveChanges();
    }

    public void RemoveItem(int userId, int productId)
    {
        var cart = GetByUserId(userId);
        if (cart == null) return;
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null) cart.Items.Remove(item);
        _db.SaveChanges();
    }

    public void Clear(int userId)
    {
        var cart = GetByUserId(userId);
        if (cart == null) return;
        cart.Items.Clear();
        _db.SaveChanges();
    }

    public void Update(Cart cart)
    {
        _db.Carts.Update(cart);
        _db.SaveChanges();
    }
}
}
