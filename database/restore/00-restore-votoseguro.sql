USE master;
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'usr_voter_service')
    CREATE LOGIN usr_voter_service WITH PASSWORD = 'VoterService2026!@';
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'usr_referendum_service')
    CREATE LOGIN usr_referendum_service WITH PASSWORD = 'ReferendumService2026!@';
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'usr_vote_service')
    CREATE LOGIN usr_vote_service WITH PASSWORD = 'VoteService2026!@';
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'usr_result_service')
    CREATE LOGIN usr_result_service WITH PASSWORD = 'ResultService2026!@';
GO

IF DB_ID('VotoSeguroVoterDb') IS NULL
BEGIN
    RESTORE DATABASE [VotoSeguroVoterDb]
    FROM DISK = N'/var/opt/mssql/backups/VotoSeguroVoterDb.bak'
    WITH 
        MOVE 'VotoSeguroVoterDb' TO '/var/opt/mssql/data/VotoSeguroVoterDb.mdf',
        MOVE 'VotoSeguroVoterDb_log' TO '/var/opt/mssql/data/VotoSeguroVoterDb_log.ldf';
END
GO

IF DB_ID('VotoSeguroReferendumDb') IS NULL
BEGIN
    RESTORE DATABASE [VotoSeguroReferendumDb]
    FROM DISK = N'/var/opt/mssql/backups/VotoSeguroReferendumDb.bak'
    WITH 
        MOVE 'VotoSeguroReferendumDb' TO '/var/opt/mssql/data/VotoSeguroReferendumDb.mdf',
        MOVE 'VotoSeguroReferendumDb_log' TO '/var/opt/mssql/data/VotoSeguroReferendumDb_log.ldf';
END
GO

IF DB_ID('VotoSeguroVoteDb') IS NULL
BEGIN
    RESTORE DATABASE [VotoSeguroVoteDb]
    FROM DISK = N'/var/opt/mssql/backups/VotoSeguroVoteDb.bak'
    WITH 
        MOVE 'VotoSeguroVoteDb' TO '/var/opt/mssql/data/VotoSeguroVoteDb.mdf',
        MOVE 'VotoSeguroVoteDb_log' TO '/var/opt/mssql/data/VotoSeguroVoteDb_log.ldf';
END
GO


USE VotoSeguroVoterDb;
GO
ALTER USER usr_voter_service WITH LOGIN = usr_voter_service;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::voter TO usr_voter_service;
GO

USE VotoSeguroReferendumDb;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[referendum].[ReferendumQuestionCandidates]') AND type in (N'U'))
BEGIN
    CREATE TABLE [referendum].[ReferendumQuestionCandidates](
        [IdCandidate] [int] IDENTITY(1,1) NOT NULL,
        [IdQuestion] [int] NOT NULL,
        [Nombre] [nvarchar](200) NOT NULL,
        [ImagenUrl] [nvarchar](1000) NULL,
        CONSTRAINT [PK_ReferendumQuestionCandidates] PRIMARY KEY CLUSTERED ([IdCandidate] ASC),
        CONSTRAINT [FK_ReferendumQuestionCandidates_ReferendumQuestions_IdQuestion] FOREIGN KEY([IdQuestion]) REFERENCES [referendum].[ReferendumQuestions] ([IdQuestion]) ON DELETE CASCADE
    );
END
GO

ALTER USER usr_referendum_service WITH LOGIN = usr_referendum_service;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::referendum TO usr_referendum_service;
GO

USE VotoSeguroVoteDb;
GO
ALTER USER usr_vote_service WITH LOGIN = usr_vote_service;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::vote TO usr_vote_service;
GO
