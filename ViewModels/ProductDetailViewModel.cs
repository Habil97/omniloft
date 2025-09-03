using Verveo.Entities;
using System.Collections.Generic;

namespace Verveo.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public bool IsFavorite { get; set; }
        public List<ProductReview> Reviews { get; set; }
        public List<ProductFeature> Features { get; set; } // Özellikler burada!
    }
}