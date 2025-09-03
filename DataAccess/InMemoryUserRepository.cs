using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public User GetById(int id) => _users.FirstOrDefault(u => u.Id == id);
        public IEnumerable<User> GetAll() => _users;
        public void Add(User user) => _users.Add(user);
        public void Update(User user)
        {
            var existing = GetById(user.Id);
            if (existing != null)
            {
                existing.Username = user.Username;
                existing.Email = user.Email;
                existing.Password = user.Password;
                existing.RoleId = user.RoleId;
                // Diğer alanlar...
            }
        }
        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
                _users.Remove(user);
        }
        public User GetByEmail(string email) => _users.FirstOrDefault(u => u.Email == email); // Ekle!
    }
}
