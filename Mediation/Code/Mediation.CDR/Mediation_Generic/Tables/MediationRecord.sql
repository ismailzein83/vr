CREATE TABLE [Mediation_Generic].[MediationRecord] (
    [EventId]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [SessionId]             NVARCHAR (400)   NULL,
    [EventTime]             DATETIME         NULL,
    [EventStatus]           TINYINT          NULL,
    [MediationDefinitionId] UNIQUEIDENTIFIER NULL,
    [EventDetails]          NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_StoreStagingRecord] PRIMARY KEY CLUSTERED ([EventId] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_MediationRecord_SessionId_MediationDefinition]
    ON [Mediation_Generic].[MediationRecord]([MediationDefinitionId] ASC, [SessionId] ASC);



