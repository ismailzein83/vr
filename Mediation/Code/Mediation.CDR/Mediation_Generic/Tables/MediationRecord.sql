CREATE TABLE [Mediation_Generic].[MediationRecord] (
    [EventId]               BIGINT           NOT NULL,
    [SessionId]             NVARCHAR (400)   NULL,
    [EventTime]             DATETIME         NULL,
    [EventStatus]           TINYINT          NULL,
    [MediationDefinitionId] UNIQUEIDENTIFIER NULL,
    [EventDetails]          NVARCHAR (MAX)   NULL,
    CONSTRAINT [IX_MediationRecord_EventId] UNIQUE NONCLUSTERED ([EventId] ASC)
);










GO
CREATE CLUSTERED INDEX [IX_MediationRecord_SessionId_MediationDefinition]
    ON [Mediation_Generic].[MediationRecord]([MediationDefinitionId] ASC, [SessionId] ASC);






GO
CREATE NONCLUSTERED INDEX [IX_MediationRecord_DefId_Status]
    ON [Mediation_Generic].[MediationRecord]([MediationDefinitionId] ASC, [EventStatus] ASC);

