using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Verveo.DataAccess
{
    public class EfCategoryRepository : ICategoryRepository
    {
        private readonly VerveoDbContext _context;
        public EfCategoryRepository(VerveoDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Category> GetAll() => _context.Categories.ToList();
        public Category GetById(int id) => _context.Categories.Find(id);
        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
        public void Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }
    }
}
