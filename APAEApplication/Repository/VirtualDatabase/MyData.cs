using Infrastructure.Models;
using Core.Enums;
using System.Collections.Generic;

namespace Infrastructure.Repository.VirtualDatabase
{
    public static class MyData
    {
        public static List<Collaborator> Collaborators = new List<Collaborator>();

        public static List<Event> Events = new List<Event>
        {
            new Event { Id = 1, Title = "Bazar Solidário APAE", Description = "Arrecadação de fundos através de venda de roupas.", Date = "20/10/2026", Status = EventStatus.Ativo },
            new Event { Id = 2, Title = "Jantar Beneficente", Description = "Jantar de confraternização anual aberto à comunidade.", Date = "15/11/2026", Status = EventStatus.Ativo }
        };
    }
}