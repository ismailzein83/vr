CREATE TABLE [NIM].[PathConnection] (
    [ID]               BIGINT   IDENTITY (1, 1) NOT NULL,
    [ConnectionID]     BIGINT   NULL,
    [PathID]           BIGINT   NULL,
    [CreatedTime]      DATETIME NULL,
    [CreatedBy]        INT      NULL,
    [LastModifiedTime] DATETIME NULL,
    [LastModifiedBy]   INT      NULL,
    CONSTRAINT [PK__PathConn__3214EC275614BF03] PRIMARY KEY CLUSTERED ([ID] ASC)
);

