CREATE TABLE [CentralLogEVT].[LogEvent] (
    [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [Message]       NVARCHAR (MAX)   NULL,
    [EventTime]     DATETIME         NULL,
    [EventTypeID]   UNIQUEIDENTIFIER NULL,
    [NodeID]        INT              NULL,
    [Details]       NVARCHAR (MAX)   NULL,
    [SourceID]      NVARCHAR (255)   NULL,
    [DataSourceID]  UNIQUEIDENTIFIER NULL,
    [ExtraFields]   NVARCHAR (MAX)   NULL,
    [EventLevelID]  UNIQUEIDENTIFIER NULL,
    [ApplicationID] INT              NULL,
    CONSTRAINT [PK_LogEvent] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_LogEvent_EventTime]
    ON [CentralLogEVT].[LogEvent]([EventTime] ASC);

