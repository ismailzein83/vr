CREATE TABLE [bp].[BPTracking] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [ParentProcessID]   BIGINT         NULL,
    [TrackingMessage]   NVARCHAR (MAX) NULL,
    [Severity]          INT            NULL,
    [EventTime]         DATETIME       NULL,
    CONSTRAINT [PK_BPTracking] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_BPTracking_Severity]
    ON [bp].[BPTracking]([Severity] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BPTracking_EventTime]
    ON [bp].[BPTracking]([EventTime] ASC);

