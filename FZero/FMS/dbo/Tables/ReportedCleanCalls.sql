CREATE TABLE [dbo].[ReportedCleanCalls] (
    [ID]              BIGINT IDENTITY (1, 1) NOT NULL,
    [GeneratedCallId] INT    NULL,
    CONSTRAINT [PK_ReportedCleanCalls] PRIMARY KEY CLUSTERED ([ID] ASC)
);

