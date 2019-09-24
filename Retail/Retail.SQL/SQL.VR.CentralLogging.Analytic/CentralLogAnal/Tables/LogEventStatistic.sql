CREATE TABLE [CentralLogAnal].[LogEventStatistic] (
    [ID]          BIGINT           NOT NULL,
    [EventType]   UNIQUEIDENTIFIER NULL,
    [Node]        INT              NULL,
    [DataSource]  UNIQUEIDENTIFIER NULL,
    [EventLevel]  UNIQUEIDENTIFIER NULL,
    [Application] INT              NULL,
    [TotalCount]  INT              NULL,
    [BatchStart]  DATETIME         NULL,
    CONSTRAINT [PK__LogEventStatistic_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);

