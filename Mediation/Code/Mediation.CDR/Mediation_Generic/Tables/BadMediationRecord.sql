CREATE TABLE [Mediation_Generic].[BadMediationRecord] (
    [EventId]               BIGINT           NOT NULL,
    [SessionId]             NVARCHAR (400)   NULL,
    [EventTime]             DATETIME         NULL,
    [EventStatus]           TINYINT          NULL,
    [MediationDefinitionId] UNIQUEIDENTIFIER NULL,
    [EventDetails]          NVARCHAR (MAX)   NULL
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_BadMediationRecord_EventId]
    ON [Mediation_Generic].[BadMediationRecord]([EventId] ASC);

