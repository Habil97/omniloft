using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public class InMemoryRoleRepository : IRoleRepository
    {
    private static List<Role> _roles = new List<Role>();
        public IEnumerable<Role> GetAll() => _roles;
        public Role GetById(int id) => _roles.FirstOrDefault(r => r.Id == id);
        public void Add(Role role) => _roles.Add(role);
        public void Update(Role role)
        {
            var existing = GetById(role.Id);
            if (existing != null)
            {
                existing.Name = role.Name;
            }
        }
        public void Delete(int id) => _roles.RemoveAll(r => r.Id == id);
    }
}
