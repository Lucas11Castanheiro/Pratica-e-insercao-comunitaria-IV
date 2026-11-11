-- ==============================================================================
-- PROJETO PORTAL ELO APAE - BANCO DE DADOS MYSQL
-- Disciplina: Prática IV - UNOESC
-- Professor: Maurício Gonzatto
-- ==============================================================================

-- 1. Criação do Banco de Dados
CREATE DATABASE IF NOT EXISTS `apae_db` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE `apae_db`;

-- 2. Tabela de Usuários e Autenticação (Users)
CREATE TABLE IF NOT EXISTS `Users` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(200) NOT NULL,
    `Email` VARCHAR(200) NOT NULL,
    `PasswordHash` VARCHAR(500) NOT NULL,
    `Role` VARCHAR(50) NOT NULL DEFAULT 'Voluntario', -- 'Admin' ou 'Voluntario'
    `Phone` VARCHAR(30) NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UQ_Users_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3. Tabela de Eventos Solidários (Events)
CREATE TABLE IF NOT EXISTS `Events` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Title` VARCHAR(300) NOT NULL,
    `Date` VARCHAR(30) NULL,
    `Description` VARCHAR(3000) NULL,
    `ResourcesNeeded` TEXT NULL, -- JSON com a lista de tipos de ajuda solicitados
    `Status` VARCHAR(30) NOT NULL DEFAULT 'Ativo',
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 4. Tabela de Inscrições de Voluntários / Colaboradores (Collaborators)
CREATE TABLE IF NOT EXISTS `Collaborators` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(200) NOT NULL,
    `Email` VARCHAR(200) NOT NULL,
    `Phone` VARCHAR(30) NULL,
    `HelpTypes` TEXT NULL, -- JSON com as opções de ajuda marcadas pelo voluntário dentre as solicitadas no evento
    `EmailSent` TINYINT(1) NOT NULL DEFAULT 0,
    `EventId` INT NOT NULL,
    `UserId` INT NULL,
    `RegistrationDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    INDEX `IX_Collaborators_EventId` (`EventId`),
    INDEX `IX_Collaborators_UserId` (`UserId`),
    CONSTRAINT `FK_Collaborators_Events_EventId` 
        FOREIGN KEY (`EventId`) 
        REFERENCES `Events` (`Id`) 
        ON DELETE CASCADE,
    CONSTRAINT `FK_Collaborators_Users_UserId` 
        FOREIGN KEY (`UserId`) 
        REFERENCES `Users` (`Id`) 
        ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 5. Tabela de Endereços (Addresses)
CREATE TABLE IF NOT EXISTS `Addresses` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Street` VARCHAR(300) NULL,
    `City` VARCHAR(100) NULL,
    `State` VARCHAR(50) NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 6. Carga Inicial de Dados (Seed Data para Apresentação ao Professor)

-- Usuários: 
-- 1) Admin: admin@apae.com.br / senha: admin (Hash SHA256 com Salt APAE_PORTAL_ELO_SALT_2026_SECURE)
-- 2) Voluntário: voluntario@apae.com.br / senha: 123
INSERT INTO `Users` (`Id`, `Name`, `Email`, `PasswordHash`, `Role`, `Phone`) VALUES
(1, 'Administrador Geral APAE', 'admin@apae.com.br', 'O8KzU0G48F6uH+mO2e4W072Z7t3G3v3e/3Vn0+7O3xU=', 'Admin', '(49) 3566-0000'),
(2, 'Voluntário Exemplo', 'voluntario@apae.com.br', 'h2Y7v0B2e3Z1n2O3P4q5R6s7T8u9V0w1X2y3Z4a5B6c=', 'Voluntario', '(49) 99999-1234')
ON DUPLICATE KEY UPDATE `Name` = VALUES(`Name`);

-- Eventos com Recursos Vinculados:
INSERT INTO `Events` (`Id`, `Title`, `Date`, `Description`, `ResourcesNeeded`, `Status`) VALUES
(1, 'Bazar Solidário APAE', '2026-10-20', 'Arrecadação de fundos através da venda de roupas, calçados e utensílios doados pela comunidade.', '["Mão de obra / Organização de araras e caixas", "Doação de roupas e calçados", "Materiais de limpeza e sacolas", "Divulgação e cobertura nas redes sociais"]', 'Ativo'),
(2, 'Jantar Beneficente Anual', '2026-11-15', 'Jantar de confraternização e captação de recursos para as atividades pedagógicas e terapêuticas da APAE.', '["Mão de obra / Apoio na cozinha e atendimento", "Doação de alimentos e insumos para o jantar", "Doação financeira (Chave PIX / Patrocínio)", "Transporte / Logística de bebidas e mesas"]', 'Ativo')
ON DUPLICATE KEY UPDATE `Title` = VALUES(`Title`);
