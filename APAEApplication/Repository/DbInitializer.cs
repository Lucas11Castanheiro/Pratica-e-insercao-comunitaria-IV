using Infrastructure.Models;
using Infrastructure.Security;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext db)
        {
            // 1. Garante que o banco de dados e as tabelas essenciais existam
            
            // Tabela de Usuários (Users)
            db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS `Users` (
                    `Id` INT NOT NULL AUTO_INCREMENT,
                    `Name` VARCHAR(200) NOT NULL,
                    `Email` VARCHAR(200) NOT NULL,
                    `PasswordHash` VARCHAR(500) NOT NULL,
                    `Role` VARCHAR(50) NOT NULL DEFAULT 'Voluntario',
                    `Phone` VARCHAR(30) NULL,
                    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (`Id`),
                    UNIQUE KEY `UQ_Users_Email` (`Email`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
            ");

            // Tabela de Eventos (Events)
            db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS `Events` (
                    `Id` INT NOT NULL AUTO_INCREMENT,
                    `Title` VARCHAR(300) NOT NULL,
                    `Date` VARCHAR(30) NULL,
                    `Description` VARCHAR(3000) NULL,
                    `ResourcesNeeded` TEXT NULL,
                    `Status` VARCHAR(30) NOT NULL DEFAULT 'Ativo',
                    PRIMARY KEY (`Id`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
            ");

            // Tabela de Voluntários / Colaboradores (Collaborators)
            db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS `Collaborators` (
                    `Id` INT NOT NULL AUTO_INCREMENT,
                    `Name` VARCHAR(200) NOT NULL,
                    `Email` VARCHAR(200) NOT NULL,
                    `Phone` VARCHAR(30) NULL,
                    `HelpTypes` TEXT NULL,
                    `EmailSent` TINYINT(1) NOT NULL DEFAULT 0,
                    `EventId` INT NOT NULL,
                    `UserId` INT NULL,
                    `RegistrationDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (`Id`),
                    INDEX `IX_Collaborators_EventId` (`EventId`),
                    INDEX `IX_Collaborators_UserId` (`UserId`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
            ");

            // 2. Verifica previamente no information_schema se as colunas já existem antes de rodar ALTER TABLE
            // Isso evita completamente o erro de coluna duplicada e os logs de fail no EF Core
            if (!ColumnExists(db, "Collaborators", "UserId"))
            {
                db.Database.ExecuteSqlRaw("ALTER TABLE `Collaborators` ADD COLUMN `UserId` INT NULL;");
            }

            if (!ColumnExists(db, "Collaborators", "RegistrationDate"))
            {
                db.Database.ExecuteSqlRaw("ALTER TABLE `Collaborators` ADD COLUMN `RegistrationDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;");
            }

            // 3. Normalização automática de dados legados no MySQL (converte arrays numéricos antigos [0], [1,2] para strings)
            try
            {
                db.Database.ExecuteSqlRaw("UPDATE `Collaborators` SET `HelpTypes` = '[\"Mão de obra\", \"Doação financeira\"]' WHERE `HelpTypes` = '[1,2]' OR `HelpTypes` = '[1, 2]';");
                db.Database.ExecuteSqlRaw("UPDATE `Collaborators` SET `HelpTypes` = '[\"Armazenamento\"]' WHERE `HelpTypes` = '[0]';");
                db.Database.ExecuteSqlRaw("UPDATE `Collaborators` SET `HelpTypes` = '[\"Mão de obra\"]' WHERE `HelpTypes` = '[1]';");
                db.Database.ExecuteSqlRaw("UPDATE `Collaborators` SET `HelpTypes` = '[\"Doação financeira\"]' WHERE `HelpTypes` = '[2]';");
                db.Database.ExecuteSqlRaw("UPDATE `Collaborators` SET `HelpTypes` = '[\"Outros\"]' WHERE `HelpTypes` = '[3]';");
                db.Database.ExecuteSqlRaw("UPDATE `Collaborators` SET `HelpTypes` = '[]' WHERE `HelpTypes` IS NULL OR `HelpTypes` = '';");
            }
            catch { }

            // Tabela de Endereços (Addresses)
            db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS `Addresses` (
                    `Id` INT NOT NULL AUTO_INCREMENT,
                    `Street` VARCHAR(300) NULL,
                    `City` VARCHAR(100) NULL,
                    `State` VARCHAR(50) NULL,
                    PRIMARY KEY (`Id`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
            ");

            // 4. Carga Inicial de Usuários (Seed de Demonstração para Avaliação)
            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User
                    {
                        Name = "Administrador Geral APAE",
                        Email = "admin@apae.com.br",
                        PasswordHash = PasswordHasher.HashPassword("admin"),
                        Role = "Admin",
                        Phone = "(49) 3566-0000"
                    },
                    new User
                    {
                        Name = "Voluntário Exemplo",
                        Email = "voluntario@apae.com.br",
                        PasswordHash = PasswordHasher.HashPassword("123"),
                        Role = "Voluntario",
                        Phone = "(49) 99999-1234"
                    }
                );
                db.SaveChanges();
            }

            // 5. Carga Inicial de Eventos com Recursos Vinculados
            if (!db.Events.Any())
            {
                db.Events.AddRange(
                    new Event
                    {
                        Title = "Bazar Solidário APAE",
                        Description = "Arrecadação de fundos através da comercialização de roupas, calçados e utensílios doados pela comunidade.",
                        Date = "2026-10-20",
                        Status = EventStatus.Ativo,
                        ResourcesNeeded = new List<string>
                        {
                            "Mão de obra / Organização de araras e caixas",
                            "Doação de roupas e calçados",
                            "Materiais de limpeza e sacolas",
                            "Divulgação e cobertura nas redes sociais"
                        }
                    },
                    new Event
                    {
                        Title = "Jantar Beneficente Anual",
                        Description = "Jantar tradicional de confraternização e captação de recursos para as atividades pedagógicas e terapêuticas da APAE.",
                        Date = "2026-11-15",
                        Status = EventStatus.Ativo,
                        ResourcesNeeded = new List<string>
                        {
                            "Mão de obra / Apoio na cozinha e atendimento",
                            "Doação de alimentos e insumos para o jantar",
                            "Doação financeira (Chave PIX / Patrocínio)",
                            "Transporte / Logística de bebidas e mesas"
                        }
                    }
                );
                db.SaveChanges();
            }
        }

        private static bool ColumnExists(AppDbContext db, string tableName, string columnName)
        {
            try
            {
                var conn = db.Database.GetDbConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT COUNT(*) 
                    FROM information_schema.COLUMNS 
                    WHERE TABLE_SCHEMA = DATABASE() 
                      AND TABLE_NAME = @tableName 
                      AND COLUMN_NAME = @columnName";

                var pTable = cmd.CreateParameter();
                pTable.ParameterName = "@tableName";
                pTable.Value = tableName;
                cmd.Parameters.Add(pTable);

                var pColumn = cmd.CreateParameter();
                pColumn.ParameterName = "@columnName";
                pColumn.Value = columnName;
                cmd.Parameters.Add(pColumn);

                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
