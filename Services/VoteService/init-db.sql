USE master;
GO

IF DB_ID('VotoSeguroVoteDb') IS NOT NULL
    DROP DATABASE VotoSeguroVoteDb;
GO

CREATE DATABASE VotoSeguroVoteDb;
GO

USE VotoSeguroVoteDb;
GO

-- ================================================
-- TABLA: TipoVoto
-- ================================================

CREATE TABLE TipoVoto (
    IdTipoVoto INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(20) NOT NULL UNIQUE
);

GO

-- ================================================
-- TABLA: Votos (ANÓNIMO — sin relación directa votante)
-- ================================================

CREATE TABLE Votos (
    IdVoto INT IDENTITY(1,1) PRIMARY KEY,
    IdVotacion INT NOT NULL,
    IdTipoVoto INT NOT NULL,
    Fecha DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT fk_voto_tipo 
        FOREIGN KEY (IdTipoVoto) 
        REFERENCES TipoVoto(IdTipoVoto)
);

GO

-- ================================================
-- TABLA: Participacion (Controla: votante ya votó, sin exponer qué votó)
-- ================================================

CREATE TABLE Participacion (
    IdParticipacion INT IDENTITY(1,1) PRIMARY KEY,
    IdVotante INT NOT NULL,
    IdVotacion INT NOT NULL,
    Fecha DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(IdVotante, IdVotacion)
);

GO

-- ================================================
-- ÍNDICES
-- ================================================

CREATE INDEX IX_Votos_IdVotacion ON Votos(IdVotacion);
CREATE INDEX IX_Votos_IdTipoVoto ON Votos(IdTipoVoto);
CREATE INDEX IX_Votos_Fecha ON Votos(Fecha);
CREATE INDEX IX_Participacion_IdVotante ON Participacion(IdVotante);
CREATE INDEX IX_Participacion_IdVotacion ON Participacion(IdVotacion);

GO

-- ================================================
-- USUARIO SQL
-- ================================================

CREATE LOGIN usr_vote_service WITH PASSWORD = 'VoteService2026!@';
GO

CREATE USER usr_vote_service FOR LOGIN usr_vote_service;
GO

GRANT SELECT, INSERT ON dbo.Votos TO usr_vote_service;
GRANT SELECT, INSERT ON dbo.Participacion TO usr_vote_service;
GRANT SELECT ON dbo.TipoVoto TO usr_vote_service;
GO

-- ================================================
-- DATOS DE PRUEBA
-- ================================================

INSERT INTO TipoVoto (Nombre) VALUES ('SI'), ('NO'), ('BLANCO'), ('NULO');

GO

-- Votos demo (anónimos)
INSERT INTO Votos (IdVotacion, IdTipoVoto) VALUES
(1, 1),  -- Un voto SI
(1, 1),  -- Otro voto SI
(1, 2),  -- Un voto NO
(1, 3),  -- Un voto BLANCO
(2, 1);  -- Voto SI en votación pasada

-- Participación (quién votó, pero no qué)
INSERT INTO Participacion (IdVotante, IdVotacion) VALUES
(1, 1),  -- Juan votó en votación 1
(2, 1),  -- Abigail votó en votación 1
(3, 1);  -- Maria votó en votación 1

GO

SELECT * FROM TipoVoto;
SELECT * FROM Votos;
SELECT * FROM Participacion;