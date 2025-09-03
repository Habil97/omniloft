using System.Collections.Generic;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public interface IUserRepository
    {
        User GetById(int id);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(int id);
        User GetByEmail(string email); // Ekle!
    }
}
