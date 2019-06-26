CREATE TABLE [NIM_PSTN].[OLTSplitterLogicalConnector] (
    [Id]                BIGINT     IDENTITY (1, 1) NOT NULL,
    [OLTHorizontalPort] BIGINT     NULL,
    [SplitterOutPort]   BIGINT     NULL,
    [CreatedBy]         INT        NULL,
    [CreatedTime]       DATETIME   NULL,
    [LastModifiedBy]    INT        NULL,
    [LastModifiedTime]  DATETIME   NULL,
    [timestamp]         ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_OLTSplitterLogicalConnector_OLTHorizontalPort] UNIQUE NONCLUSTERED ([OLTHorizontalPort] ASC),
    CONSTRAINT [IX_OLTSplitterLogicalConnector_SplitterOutPort] UNIQUE NONCLUSTERED ([SplitterOutPort] ASC)
);

