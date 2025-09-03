using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public class InMemoryProductRepository : IProductRepository
    {
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Apple 13 Pro Max Mavi 128 GB", Description = "Modern akıllı telefon.", Price = 40798.80M, CategoryId = 1, ImagePath = "sample1.jpg", IsActive = true, IsFeatured = true }
        };
        public IEnumerable<Product> GetAll() => _products;
        public Product GetById(int id) => _products.FirstOrDefault(p => p.Id == id);
        public void Add(Product product) => _products.Add(product);
        public void Update(Product product)
        {
            var existing = GetById(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.CategoryId = product.CategoryId;
                existing.ImagePath = product.ImagePath;
                existing.IsActive = product.IsActive;
                existing.IsFeatured = product.IsFeatured;
            }
        }
        public void Delete(int id) => _products.RemoveAll(p => p.Id == id);
    }
}
