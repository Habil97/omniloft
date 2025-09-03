using System.Collections.Generic;
using Verveo.Entities;

public class FavoriteProductService
{
    private readonly IFavoriteProductRepository _repo;
    public FavoriteProductService(IFavoriteProductRepository repo) { _repo = repo; }

    public List<FavoriteProduct> GetFavorites(int userId) => _repo.GetByUserId(userId);
    public void AddFavorite(int userId, int productId)
        => _repo.Add(new FavoriteProduct { UserId = userId, ProductId = productId, CreatedAt = DateTime.Now });
    public void RemoveFavorite(int userId, int productId) => _repo.Remove(userId, productId);
    public bool IsFavorite(int userId, int productId) => _repo.Exists(userId, productId);
}