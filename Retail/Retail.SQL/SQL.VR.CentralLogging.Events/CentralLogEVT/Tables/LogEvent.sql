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
    CONSTRAINT [PK__LogEvent__3214EC277F60ED59] PRIMARY KEY CLUSTERED ([ID] ASC)
);

