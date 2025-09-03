using System.Collections.Generic;
using Verveo.Entities;

namespace Verveo.Models
{
    public class AdminPanelViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<User> Users { get; set; } = new List<User>();
        public List<Role> Roles { get; set; } = new List<Role>();
        public List<Slider> Sliders { get; set; } = new List<Slider>();
    }
}
