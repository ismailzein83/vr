CREATE TABLE [reprocess].[StagingSummaryRecord] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceId] BIGINT         NOT NULL,
    [StageName]         NVARCHAR (255) NOT NULL,
    [BatchStart]        DATETIME       NOT NULL,
    [Data]              VARCHAR (MAX)  NOT NULL,
    CONSTRAINT [PK_StagingSummaryRecord] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_StagingSummaryRecord_ProcessId_StageName]
    ON [reprocess].[StagingSummaryRecord]([ProcessInstanceId] ASC, [StageName] ASC);

