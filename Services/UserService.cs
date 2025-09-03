using System.Collections.Generic;
using Verveo.Entities;
using Verveo.DataAccess;
using BCrypt.Net;

namespace Verveo.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetById(int id) => _userRepository.GetById(id);
        public User GetByEmail(string email) => _userRepository.GetByEmail(email);
        public IEnumerable<User> GetAllUsers() => _userRepository.GetAll();
        public void AddUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _userRepository.Add(user);
        }
        public void UpdateUser(User user) => _userRepository.Update(user);
        public void DeleteUser(int id) => _userRepository.Delete(id);

        // DÜZELTME: AdminController'da kullanılan GetAll için alias ekle
        public IEnumerable<User> GetAll() => GetAllUsers();

        public User Authenticate(string email, string password)
        {
            // Kullanıcıyı veritabanından bul ve şifreyi doğrula
            var user = _userRepository.GetByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            return user;
        }

        public void UpdatePassword(int userId, string newPassword)
        {
            var user = _userRepository.GetById(userId);
            if (user != null)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword); // Şifreyi hashle
                _userRepository.Update(user);
            }
        }
    }
}
