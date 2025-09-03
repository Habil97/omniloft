using System.Collections.Generic;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public interface ICartRepository
{
    Cart GetByUserId(int userId);
    void AddItem(int userId, int productId, int quantity);
    void Update(Cart cart);
    void Clear(int userId);
}
}
