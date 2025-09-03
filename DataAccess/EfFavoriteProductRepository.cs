using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;
using Verveo.DataAccess;
using Microsoft.EntityFrameworkCore;

public class EfFavoriteProductRepository : IFavoriteProductRepository
{
    private readonly VerveoDbContext _db;
    public EfFavoriteProductRepository(VerveoDbContext db) { _db = db; }

    public List<FavoriteProduct> GetByUserId(int userId)
        => _db.FavoriteProducts.Include(f => f.Product).Where(f => f.UserId == userId).ToList();

    public void Add(FavoriteProduct favorite)
    {
        if (!Exists(favorite.UserId, favorite.ProductId))
        {
            _db.FavoriteProducts.Add(favorite);
            _db.SaveChanges();
        }
    }

    public void Remove(int userId, int productId)
    {
        var fav = _db.FavoriteProducts.FirstOrDefault(f => f.UserId == userId && f.ProductId == productId);
        if (fav != null)
        {
            _db.FavoriteProducts.Remove(fav);
            _db.SaveChanges();
        }
    }

    public bool Exists(int userId, int productId)
        => _db.FavoriteProducts.Any(f => f.UserId == userId && f.ProductId == productId);
}