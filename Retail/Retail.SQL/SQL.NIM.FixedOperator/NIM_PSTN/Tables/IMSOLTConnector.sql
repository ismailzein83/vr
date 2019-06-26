CREATE TABLE [NIM_PSTN].[IMSOLTConnector] (
    [Id]                BIGINT     IDENTITY (1, 1) NOT NULL,
    [TID]               BIGINT     NULL,
    [OLTHorizontalPort] BIGINT     NULL,
    [CreatedBy]         INT        NULL,
    [CreatedTime]       DATETIME   NULL,
    [LastModifiedBy]    INT        NULL,
    [LastModifiedTime]  DATETIME   NULL,
    [timestamp]         ROWVERSION NULL,
    CONSTRAINT [PK__IMSOLTCo__3214EC0740C49C62] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_IMSOLTConnector_OLTPort] UNIQUE NONCLUSTERED ([OLTHorizontalPort] ASC),
    CONSTRAINT [IX_IMSOLTConnector_TID] UNIQUE NONCLUSTERED ([TID] ASC)
);



