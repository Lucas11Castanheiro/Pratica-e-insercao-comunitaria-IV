using Infrastructure.Models;
using Infrastructure.Repository.VirtualDatabase;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public class CollaboratorRepository
    {
        public void Create(Collaborator collaborator)
        {
            collaborator.Id = MyData.Collaborators.Count > 0 ? MyData.Collaborators.Max(c => c.Id) + 1 : 1;
            MyData.Collaborators.Add(collaborator);
        }

        public List<Collaborator> GetAll()
        {
            return MyData.Collaborators;
        }

        public void ToggleEmailStatus(int id)
        {
            var collaborator = MyData.Collaborators.FirstOrDefault(c => c.Id == id);
            if (collaborator != null)
            {
                // Inverte o estado atual (se for true, vira false; se for false, vira true)
                collaborator.EmailSent = !collaborator.EmailSent;
            }
        }

    }
}