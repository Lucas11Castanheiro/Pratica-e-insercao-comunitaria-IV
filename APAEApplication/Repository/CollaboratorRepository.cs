using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public class CollaboratorRepository
    {
        private readonly AppDbContext _context;

        public CollaboratorRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Create(Collaborator collaborator)
        {
            _context.Collaborators.Add(collaborator);
            _context.SaveChanges();
        }

        public List<Collaborator> GetAll()
        {
            return _context.Collaborators
                .Include(c => c.Event)
                .OrderByDescending(c => c.RegistrationDate)
                .ToList();
        }

        public Collaborator? GetById(int id)
        {
            return _context.Collaborators
                .Include(c => c.Event)
                .FirstOrDefault(c => c.Id == id);
        }

        public List<Collaborator> GetByUserId(int userId)
        {
            return _context.Collaborators
                .Include(c => c.Event)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.RegistrationDate)
                .ToList();
        }

        public List<Collaborator> GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return new List<Collaborator>();
            var normalizedEmail = email.Trim().ToLower();

            return _context.Collaborators
                .Include(c => c.Event)
                .Where(c => c.Email.ToLower() == normalizedEmail)
                .OrderByDescending(c => c.RegistrationDate)
                .ToList();
        }

        public void ToggleEmailStatus(int id)
        {
            var collaborator = _context.Collaborators.FirstOrDefault(c => c.Id == id);
            if (collaborator != null)
            {
                collaborator.EmailSent = !collaborator.EmailSent;
                _context.SaveChanges();
            }
        }

        public bool CancelRegistration(int id, int? userId, string? userEmail)
        {
            var collaborator = _context.Collaborators.FirstOrDefault(c => c.Id == id);
            if (collaborator == null) return false;

            // Garante que só o próprio usuário ou admin pode cancelar
            if (userId.HasValue && collaborator.UserId == userId.Value)
            {
                _context.Collaborators.Remove(collaborator);
                _context.SaveChanges();
                return true;
            }

            if (!string.IsNullOrEmpty(userEmail) && collaborator.Email.Equals(userEmail.Trim(), System.StringComparison.OrdinalIgnoreCase))
            {
                _context.Collaborators.Remove(collaborator);
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public void DeleteByEventId(int eventId)
        {
            var collaborators = _context.Collaborators.Where(c => c.EventId == eventId).ToList();
            _context.Collaborators.RemoveRange(collaborators);
            _context.SaveChanges();
        }
    }
}
