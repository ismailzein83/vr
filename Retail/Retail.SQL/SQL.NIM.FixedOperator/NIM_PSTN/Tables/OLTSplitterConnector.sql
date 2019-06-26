CREATE TABLE [NIM_PSTN].[OLTSplitterConnector] (
    [Id]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [OLTVerticalPort]  BIGINT     NULL,
    [SplitterInPort]   BIGINT     NULL,
    [CreatedBy]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    CONSTRAINT [PK__OLT_SDF___3214EC0700DF2177] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_OLTSplitterConnector_OLTPort] UNIQUE NONCLUSTERED ([OLTVerticalPort] ASC),
    CONSTRAINT [IX_OLTSplitterConnector_SplitterPort] UNIQUE NONCLUSTERED ([SplitterInPort] ASC)
);



