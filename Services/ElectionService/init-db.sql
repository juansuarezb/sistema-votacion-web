USE master;
GO

IF DB_ID('VotoSeguroElectionDb') IS NOT NULL
    DROP DATABASE VotoSeguroElectionDb;
GO

CREATE DATABASE VotoSeguroElectionDb;
GO

USE VotoSeguroElectionDb;
GO

-- ================================================
-- TABLA: Votaciones
-- ================================================

CREATE TABLE Votaciones (
    IdVotacion INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(200) NOT NULL,
    Descripcion NVARCHAR(MAX),
    FechaInicio DATE NOT NULL,
    FechaCierre DATE NOT NULL,
    IdAdmin NVARCHAR(255) NOT NULL,  -- Azure AD Object ID del administrador
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT chk_fechas CHECK (FechaCierre > FechaInicio)
);

GO

-- ================================================
-- TABLA: Votacion_Votante (Padrón electoral)
-- ================================================

CREATE TABLE Votacion_Votante (
    IdVotacion INT NOT NULL,
    IdVotante INT NOT NULL,
    HaVotado BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (IdVotacion, IdVotante),
    CONSTRAINT fk_votacion_votante 
        FOREIGN KEY (IdVotacion) 
        REFERENCES Votaciones(IdVotacion) 
        ON DELETE CASCADE
);

GO

-- ================================================
-- ÍNDICES
-- ================================================

CREATE INDEX IX_Votaciones_FechaInicio ON Votaciones(FechaInicio);
CREATE INDEX IX_Votaciones_FechaCierre ON Votaciones(FechaCierre);
CREATE INDEX IX_Votaciones_IdAdmin ON Votaciones(IdAdmin);
CREATE INDEX IX_VotacionVotante_Votacion ON Votacion_Votante(IdVotacion);
CREATE INDEX IX_VotacionVotante_Votante ON Votacion_Votante(IdVotante);

GO

-- ================================================
-- USUARIO SQL
-- ================================================

CREATE LOGIN usr_election_service WITH PASSWORD = 'ElectionService2026!@';
GO

CREATE USER usr_election_service FOR LOGIN usr_election_service;
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Votaciones TO usr_election_service;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Votacion_Votante TO usr_election_service;
GO

-- ================================================
-- DATOS DE PRUEBA
-- ================================================

INSERT INTO Votaciones (Titulo, Descripcion, FechaInicio, FechaCierre, IdAdmin) VALUES
('Elección Consejo Estudiantil', 'Periodo 2026 - Elección de representantes', 
 CAST(GETDATE() AS DATE), CAST(DATEADD(DAY, 30, GETDATE()) AS DATE), 'admin-uuid-1'),
('Consulta Presupuestaria', 'Aprobación de presupuesto anual', 
 CAST(DATEADD(DAY, -60, GETDATE()) AS DATE), CAST(DATEADD(DAY, -30, GETDATE()) AS DATE), 'admin-uuid-1');

GO

SELECT * FROM Votaciones;