using System;
using System.Collections.Generic;

namespace Verveo.Entities
{
    public class Product
    {
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? ImagePath { get; set; } // Yüklenen dosyanın adı/yolu
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public int Stock { get; set; } 
    public ICollection<ProductFeature> Features { get; set; }
    public ICollection<ProductImage> Images { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public ICollection<ProductReview> Reviews { get; set; } // Ekle!
    }
}
