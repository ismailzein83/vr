CREATE TABLE [bp].[BPVisualEvent] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID] BIGINT           NOT NULL,
    [ActivityID]        UNIQUEIDENTIFIER NOT NULL,
    [Title]             NVARCHAR (MAX)   NULL,
    [EventTypeID]       UNIQUEIDENTIFIER NOT NULL,
    [EventPayload]      NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]       DATETIME         CONSTRAINT [DF_BPVisualEvent_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BPVisualEvent] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_BPVisualEvent_ProcessInstanceAndId]
    ON [bp].[BPVisualEvent]([ProcessInstanceID] ASC, [ID] ASC);

