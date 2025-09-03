using System.Collections.Generic;
using Verveo.Entities;

namespace Verveo.Models
{
    public class HomeViewModel
    {
        public List<Verveo.Entities.Product> FeaturedProducts { get; set; } = new();
        public List<Verveo.Entities.Category> Categories { get; set; } = new();
        public List<Slider> Sliders { get; set; }
    }
}
