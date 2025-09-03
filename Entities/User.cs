using System;
using System.Collections.Generic;

namespace Verveo.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } // Kullanıcı adı
        public string Email { get; set; }    // E-posta
        public string Password { get; set; }
        public int RoleId { get; set; }
        // Diğer özellikler...
        public ICollection<FavoriteProduct> FavoriteProducts { get; set; }
        public ICollection<ProductReview> Reviews { get; set; }
    }
}
