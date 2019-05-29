CREATE TABLE [NIM].[SplitterFDBConnector] (
    [Id]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [SplitterOutPort]  BIGINT     NULL,
    [FDBPort]          BIGINT     NULL,
    [CreatedBy]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

