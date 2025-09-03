using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;
using Verveo.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Verveo.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly VerveoDbContext _dbContext;

        public ProductService(IProductRepository productRepository, VerveoDbContext dbContext)
        {
            _productRepository = productRepository;
            _dbContext = dbContext;
        }

        public IEnumerable<Product> GetAllProducts() => _productRepository.GetAll();
        public Product GetById(int id) => _productRepository.GetById(id);
        public IEnumerable<Product> GetAll() => _productRepository.GetAll();
        public void AddProduct(Product product) => _productRepository.Add(product);
        public void UpdateProduct(Product product) => _productRepository.Update(product);
        public void DeleteProduct(int id) => _productRepository.Delete(id);

        public List<Product> GetFeaturedProducts()
        {
            return _productRepository.GetAll()
                .Where(p => p.Stock > 0)
                .Take(8)
                .ToList();
        }

        public Product GetProductWithFeatures(int productId)
        {
            return _dbContext.Products
                .Include(p => p.Features)
                .FirstOrDefault(p => p.Id == productId);
        }

        public void UpdateProductFeatures(int productId, List<ProductFeature> features)
        {
            var product = _dbContext.Products.Include(p => p.Features).FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                product.Features.Clear();
                foreach (var feature in features)
                {
                    product.Features.Add(new ProductFeature
                    {
                        Name = feature.Name,
                        Value = feature.Value,
                        ProductId = productId
                    });
                }
                _dbContext.SaveChanges();
            }
        }

        public List<ProductFeature> GetFeatures(int productId)
        {
            return _dbContext.ProductFeature
                .Where(f => f.ProductId == productId)
                .ToList();
        }
    }
}
