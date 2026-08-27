using Infrastructure.Models;
using Infrastructure.Repository.VirtualDatabase;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public class EventRepository
    {
        public void Create(Event newEvent)
        {
            newEvent.Id = MyData.Events.Count > 0 ? MyData.Events.Max(e => e.Id) + 1 : 1;
            MyData.Events.Add(newEvent);
        }

        public List<Event> GetAll()
        {
            return MyData.Events;
        }

        public Event? GetById(int id)
        {
            return MyData.Events.FirstOrDefault(e => e.Id == id);
        }

        // Adicione estes métodos logo abaixo do GetById

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
            }
        }

        public void Delete(int id)
        {
            var existingEvent = GetById(id);
            if (existingEvent != null)
            {
                // Exclui o evento
                MyData.Events.Remove(existingEvent);

                // Exclusão em Cascata: Remove todos os colaboradores atrelados ao evento excluído
                MyData.Collaborators.RemoveAll(c => c.EventId == id);
            }
        }
    }
}