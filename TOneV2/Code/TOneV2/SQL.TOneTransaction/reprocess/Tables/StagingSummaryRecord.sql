CREATE TABLE [reprocess].[StagingSummaryRecord] (
    [ProcessInstanceId] BIGINT         NOT NULL,
    [StageName]         NVARCHAR (255) NOT NULL,
    [BatchStart]        DATETIME       NOT NULL,
    [BatchEnd]          DATETIME       NOT NULL,
    [Data]              VARCHAR (MAX)  NULL,
    [AlreadyFinalised]  BIT            NOT NULL,
    [CreatedTime]       DATETIME       CONSTRAINT [DF_StagingSummaryRecord_CreatedTime] DEFAULT (getdate()) NOT NULL
);










GO
CREATE CLUSTERED INDEX [IX_StagingSummaryRecord_ProcessId_StageName_BatchStart]
    ON [reprocess].[StagingSummaryRecord]([ProcessInstanceId] ASC, [StageName] ASC, [BatchStart] ASC);

