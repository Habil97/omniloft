using System.Collections.Generic;
using Verveo.Entities;
using Verveo.DataAccess;

namespace Verveo.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public IEnumerable<Role> GetAll()
{
    return _roleRepository.GetAll();
}

        public IEnumerable<Role> GetAllRoles() => _roleRepository.GetAll();
        public Role GetRoleById(int id) => _roleRepository.GetById(id);
        public void AddRole(Role role) => _roleRepository.Add(role);
        public void UpdateRole(Role role) => _roleRepository.Update(role);
        public void DeleteRole(int id) => _roleRepository.Delete(id);
    }
}
