using System.Collections.Generic;
using Verveo.Entities;
using Verveo.DataAccess;

namespace Verveo.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryRepository.GetAll();
        }

        public Category GetCategoryById(int id) => _categoryRepository.GetById(id);
        public void AddCategory(Category category) => _categoryRepository.Add(category);
        public void UpdateCategory(Category category) => _categoryRepository.Update(category);
        public void DeleteCategory(int id) => _categoryRepository.Delete(id);
    }
}
