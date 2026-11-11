using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public class EventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Create(Event newEvent)
        {
            _context.Events.Add(newEvent);
            _context.SaveChanges();
        }

        public List<Event> GetAll()
        {
            return _context.Events.OrderBy(e => e.Date).ToList();
        }

        public Event? GetById(int id)
        {
            return _context.Events.FirstOrDefault(e => e.Id == id);
        }

        public void Update(Event updatedEvent)
        {
            var existingEvent = GetById(updatedEvent.Id);
            if (existingEvent != null)
            {
                existingEvent.Title = updatedEvent.Title;
                existingEvent.Date = updatedEvent.Date;
                existingEvent.Description = updatedEvent.Description;
                existingEvent.ResourcesNeeded = updatedEvent.ResourcesNeeded;
                existingEvent.Status = updatedEvent.Status;
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var existingEvent = GetById(id);
            if (existingEvent != null)
            {
                // Remove colaboradores vinculados
                var collaborators = _context.Collaborators.Where(c => c.EventId == id).ToList();
                _context.Collaborators.RemoveRange(collaborators);

                // Exclui o evento
                _context.Events.Remove(existingEvent);
                _context.SaveChanges();
            }
        }
    }
}
