CREATE TABLE [NIM_PSTN].[SplitterInOutConnector] (
    [Id]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [InPort]           BIGINT     NULL,
    [OutPort]          BIGINT     NULL,
    [CreatedBy]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_SplitterInOutConnector_InPort] UNIQUE NONCLUSTERED ([InPort] ASC),
    CONSTRAINT [IX_SplitterInOutConnector_OutPort] UNIQUE NONCLUSTERED ([OutPort] ASC)
);



