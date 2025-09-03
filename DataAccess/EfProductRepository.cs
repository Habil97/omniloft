using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Verveo.DataAccess
{
    public class EfProductRepository : IProductRepository
    {
        private readonly VerveoDbContext _context;
        public EfProductRepository(VerveoDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Product> GetAll() => _context.Products.ToList();
        public Product GetById(int id) => _context.Products.Find(id);
        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }
        public void Update(Product product)
        {
            var tracked = _context.Products.Local.FirstOrDefault(p => p.Id == product.Id);
            if (tracked != null)
            {
                _context.Entry(tracked).State = EntityState.Detached;
            }
            _context.Products.Update(product);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
    }
}
