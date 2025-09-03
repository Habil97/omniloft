using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Verveo.DataAccess
{
    public class EfUserRepository : IUserRepository
    {
        private readonly VerveoDbContext _dbContext;
        public EfUserRepository(VerveoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User GetById(int id) => _dbContext.Users.Find(id);
        public IEnumerable<User> GetAll() => _dbContext.Users.ToList();
        public void Add(User user) { _dbContext.Users.Add(user); _dbContext.SaveChanges(); }
        public void Update(User user) { _dbContext.Users.Update(user); _dbContext.SaveChanges(); }
        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
            }
        }
        public User GetByEmail(string email) => _dbContext.Users.FirstOrDefault(u => u.Email == email); // Ekle!
    }
}
