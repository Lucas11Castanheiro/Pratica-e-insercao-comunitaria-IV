using Core.Enums;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public class Collaborator
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Alterado para lista para suportar múltiplos Checkboxes
        public List<HelpType> HelpTypes { get; set; } = new List<HelpType>();

        public bool EmailSent { get; set; } = false;
        public int EventId { get; set; }
    }
}