using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Verveo.DataAccess
{
    public class EfRoleRepository : IRoleRepository
    {
        private readonly VerveoDbContext _context;
        public EfRoleRepository(VerveoDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Role> GetAll() => _context.Roles.ToList();
        public Role GetById(int id) => _context.Roles.Find(id);
        public void Add(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
        }
        public void Update(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var role = _context.Roles.Find(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();
            }
        }
    }
}
