using System.Collections.Generic;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public interface IRoleRepository
    {
        IEnumerable<Role> GetAll();
        Role GetById(int id);
        void Add(Role role);
        void Update(Role role);
        void Delete(int id);
    }
}
