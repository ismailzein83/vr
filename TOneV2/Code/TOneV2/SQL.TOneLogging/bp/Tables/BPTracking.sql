CREATE TABLE [bp].[BPTracking] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [ParentProcessID]   BIGINT         NULL,
    [TrackingMessage]   NVARCHAR (MAX) NULL,
    [ExceptionDetail]   NVARCHAR (MAX) NULL,
    [Severity]          INT            NULL,
    [EventTime]         DATETIME       NULL,
    CONSTRAINT [IX_BPTracking_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);






GO



GO
CREATE NONCLUSTERED INDEX [IX_BPTracking_Severity]
    ON [bp].[BPTracking]([Severity] ASC);


GO
CREATE CLUSTERED INDEX [IX_BPTracking_ProcessInstanceEventTime]
    ON [bp].[BPTracking]([ProcessInstanceID] ASC, [EventTime] ASC);

