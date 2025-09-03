using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Verveo.Entities;

namespace Verveo.ViewModels
{
    public class ProductFeatureViewModel
    {
        public Verveo.Entities.Product Product { get; set; }
        public List<Verveo.Entities.ProductFeature> Features { get; set; } = new List<Verveo.Entities.ProductFeature>();
        public List<IFormFile> GalleryFiles { get; set; } = new List<IFormFile>();
        public List<ProductReview> Reviews { get; set; }
    }
}