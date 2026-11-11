using Infrastructure.Models;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public User? GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public User? GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.Trim().ToLower());
        }

        public bool EmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return _context.Users.Any(u => u.Email.ToLower() == email.Trim().ToLower());
        }

        public void Create(User user, string plainPassword)
        {
            user.Email = user.Email.Trim().ToLower();
            user.PasswordHash = PasswordHasher.HashPassword(plainPassword);
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User? Authenticate(string email, string plainPassword)
        {
            var user = GetByEmail(email);
            if (user == null) return null;

            if (PasswordHasher.VerifyPassword(plainPassword, user.PasswordHash))
            {
                return user;
            }

            return null;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
