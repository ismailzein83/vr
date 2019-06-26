CREATE TABLE [NIM_PSTN].[SplitterFDBConnector] (
    [Id]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [SplitterOutPort]  BIGINT     NULL,
    [FDBPort]          BIGINT     NULL,
    [CreatedBy]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    CONSTRAINT [PK_SplitterFDBConnector] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_SplitterFDBConnector_FDBPort] UNIQUE NONCLUSTERED ([FDBPort] ASC),
    CONSTRAINT [IX_SplitterFDBConnector_SplitterPort] UNIQUE NONCLUSTERED ([SplitterOutPort] ASC)
);



