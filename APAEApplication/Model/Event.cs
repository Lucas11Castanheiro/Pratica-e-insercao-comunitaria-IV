using Core.Enums;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Alterado para lista para suportar o menu de seleção múltipla na View
        public List<string> ResourcesNeeded { get; set; } = new List<string>();

        public EventStatus Status { get; set; } = EventStatus.Ativo;
    }
}