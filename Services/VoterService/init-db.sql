USE master;
GO

IF DB_ID('VotoSeguroVoterDb') IS NOT NULL
    DROP DATABASE VotoSeguroVoterDb;
GO

CREATE DATABASE VotoSeguroVoterDb
CHARACTER SET utf8
COLLATE utf8_general_ci;
GO

USE VotoSeguroVoterDb;
GO

-- ================================================
-- TABLA: Votantes
-- ================================================

CREATE TABLE Votantes (
    IdVotante INT IDENTITY(1,1) PRIMARY KEY,
    KeycloakId NVARCHAR(255) NOT NULL UNIQUE,
    AdminId NVARCHAR(255) NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Cedula VARCHAR(10) NOT NULL UNIQUE,
    CorreoElectronico VARCHAR(100) NOT NULL UNIQUE,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

GO

-- ================================================
-- ÍNDICES
-- ================================================

CREATE INDEX IX_Votantes_Cedula ON Votantes(Cedula);
CREATE INDEX IX_Votantes_CorreoElectronico ON Votantes(CorreoElectronico);
CREATE INDEX IX_Votantes_KeycloakId ON Votantes(KeycloakId);
CREATE INDEX IX_Votantes_AdminId ON Votantes(AdminId);

GO

-- ================================================
-- USUARIO SQL
-- ================================================

CREATE LOGIN usr_voter_service WITH PASSWORD = 'VoterService2026!@';
GO

CREATE USER usr_voter_service FOR LOGIN usr_voter_service;
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Votantes TO usr_voter_service;
GO

-- ================================================
-- DATOS DE PRUEBA
-- ================================================

INSERT INTO Votantes (KeycloakId, Nombre, Cedula, CorreoElectronico) VALUES
('keycloak-uuid-1', 'Juan Perez', '1723456789', 'juan@gmail.com'),
('keycloak-uuid-2', 'Abigail Lopez', '1711111111', 'abi@gmail.com'),
('keycloak-uuid-3', 'Maria Garcia', '1733333333', 'maria@gmail.com');

GO

SELECT * FROM Votantes;