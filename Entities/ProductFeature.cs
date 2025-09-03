using System;

namespace Verveo.Entities
{
    public class ProductFeature
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }      // Özellik adı (örn: Renk, Beden, Materyal)
        public string Value { get; set; }     // Özellik değeri (örn: Kırmızı, XL, Pamuk)
        public Product Product { get; set; }
    }
}