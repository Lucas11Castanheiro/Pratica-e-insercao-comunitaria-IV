using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public class Collaborator
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Lista dinâmica dos tipos de ajuda selecionados para o evento
        public List<string> HelpTypes { get; set; } = new List<string>();

        public bool EmailSent { get; set; } = false;
        
        // Chave estrangeira e navegação para Evento
        public int EventId { get; set; }
        public Event? Event { get; set; }

        // Vínculo opcional com a conta de usuário
        public int? UserId { get; set; }
        public User? User { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
