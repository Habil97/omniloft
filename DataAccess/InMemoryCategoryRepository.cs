using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public class InMemoryCategoryRepository : ICategoryRepository
    {
        private static List<Category> _categories = new List<Category>
        {
            new Category { Id = 1, Name = "Telefon" }
        };
        public IEnumerable<Category> GetAll() => _categories;
        public Category GetById(int id) => _categories.FirstOrDefault(c => c.Id == id);
        public void Add(Category category) => _categories.Add(category);
        public void Update(Category category)
        {
            var existing = GetById(category.Id);
            if (existing != null)
            {
                existing.Name = category.Name;
            }
        }
        public void Delete(int id) => _categories.RemoveAll(c => c.Id == id);
    }
}
