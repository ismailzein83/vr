CREATE TABLE [bp].[BPTracking] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID] UNIQUEIDENTIFIER NOT NULL,
    [ParentProcessID]   UNIQUEIDENTIFIER NULL,
    [TrackingMessage]   NVARCHAR (MAX)   NULL,
    [Severity]          INT              NULL,
    [EventTime]         DATETIME         NULL,
    CONSTRAINT [PK_BPTracking] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_BPTracking_EventTime]
    ON [bp].[BPTracking]([EventTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BPTracking_Severity]
    ON [bp].[BPTracking]([Severity] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BPTracking_ParentProcessID]
    ON [bp].[BPTracking]([ParentProcessID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BPTracking_ProcessID]
    ON [bp].[BPTracking]([ProcessInstanceID] ASC);

