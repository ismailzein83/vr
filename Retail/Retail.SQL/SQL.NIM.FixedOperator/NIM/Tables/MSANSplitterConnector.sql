CREATE TABLE [NIM].[MSANSplitterConnector] (
    [Id]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [OLTPort]          BIGINT     NULL,
    [SplitterInPort]   BIGINT     NULL,
    [CreatedBy]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

