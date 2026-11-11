using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Infrastructure.Models;
using Infrastructure.Repository.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Collaborator> Collaborators { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Comparador de valor para coleções List<string> (elimina warning 10620)
            var stringListComparer = new ValueComparer<List<string>>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            // Configuração da tabela de Usuários (Users)
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Phone).HasMaxLength(30);
            });

            // Configuração da tabela de Eventos (Events)
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Events");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Date).HasMaxLength(30);
                entity.Property(e => e.Description).HasMaxLength(3000);

                // Converte List<string> de recursos necessários usando conversor resiliente
                entity.Property(e => e.ResourcesNeeded)
                    .HasConversion(
                        v => JsonListConverter.SerializeList(v),
                        v => JsonListConverter.DeserializeStringList(v)
                    )
                    .Metadata.SetValueComparer(stringListComparer);

                entity.Property(e => e.ResourcesNeeded).HasColumnType("text");

                // Converte EventStatus enum para string
                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(30);
            });

            // Configuração da tabela de Voluntários / Colaboradores (Collaborators)
            modelBuilder.Entity<Collaborator>(entity =>
            {
                entity.ToTable("Collaborators");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Phone).HasMaxLength(30);

                // Converte List<string> de tipos de ajuda selecionados usando conversor resiliente (suporta números legados e strings)
                entity.Property(c => c.HelpTypes)
                    .HasConversion(
                        v => JsonListConverter.SerializeList(v),
                        v => JsonListConverter.DeserializeHelpTypes(v)
                    )
                    .Metadata.SetValueComparer(stringListComparer);

                entity.Property(c => c.HelpTypes).HasColumnType("text");

                // Relacionamento com Evento (1:N)
                entity.HasOne(c => c.Event)
                    .WithMany()
                    .HasForeignKey(c => c.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacionamento opcional com Usuário (1:N)
                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuração da tabela de Endereços
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Addresses");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Street).HasMaxLength(300);
                entity.Property(a => a.City).HasMaxLength(100);
                entity.Property(a => a.State).HasMaxLength(50);
            });
        }
    }
}
