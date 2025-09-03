using System.Collections.Generic;
using Verveo.Entities;

public interface IFavoriteProductRepository
{
    List<FavoriteProduct> GetByUserId(int userId);
    void Add(FavoriteProduct favorite);
    void Remove(int userId, int productId);
    bool Exists(int userId, int productId);
}