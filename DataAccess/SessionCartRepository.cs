using Microsoft.AspNetCore.Http;
using Verveo.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Verveo.DataAccess
{
    public class SessionCartRepository // DÜZENLEME: ICartRepository'den miras alma kaldırıldı.
    {
        private const string CartSessionKey = "SessionCart";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly VerveoDbContext _db;

        public SessionCartRepository(IHttpContextAccessor accessor, VerveoDbContext db)
        {
            _httpContextAccessor = accessor;
            _db = db;
        }

        public Cart GetCartFromSession(ISession session)
        {
            var cartJson = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
                return new Cart { Items = new List<CartItem>() };

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Cart>(cartJson); // DÜZENLEME: JsonConvert kullanımında tam isim alanı belirtildi.
        }

        public void SaveCartToSession(ISession session, Cart cart)
        {
            var cartJson = Newtonsoft.Json.JsonConvert.SerializeObject(cart);
            session.SetString(CartSessionKey, cartJson);
        }

        public void ClearCartFromSession(ISession session)
        {
            session.Remove(CartSessionKey);
        }
    }
}