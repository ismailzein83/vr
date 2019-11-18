CREATE TABLE [NIM].[NodeLocalAreaCode] (
    [Id]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [NodeID]           BIGINT     NULL,
    [LocalAreaCodeID]  BIGINT     NULL,
    [CreatedBy]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    CONSTRAINT [PK__NodeLoca__3214EC077869D707] PRIMARY KEY CLUSTERED ([Id] ASC)
);

