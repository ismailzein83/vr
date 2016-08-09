CREATE TABLE [reprocess].[StagingSummaryRecord] (
    [ProcessInstanceId] BIGINT         NOT NULL,
    [StageName]         NVARCHAR (255) NOT NULL,
    [BatchStart]        DATETIME       NOT NULL,
    [Data]              VARCHAR (MAX)  NOT NULL
);




GO
CREATE CLUSTERED INDEX [IX_StagingSummaryRecord_ProcessId_StageName]
    ON [reprocess].[StagingSummaryRecord]([ProcessInstanceId] ASC, [StageName] ASC);



