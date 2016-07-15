CREATE TABLE [integration].[DataSourceLog] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [DataSourceId]    INT            NOT NULL,
    [Severity]        INT            NOT NULL,
    [Message]         NVARCHAR (MAX) NULL,
    [ImportedBatchId] BIGINT         NULL,
    [LogEntryTime]    DATETIME       NOT NULL,
    CONSTRAINT [IX_DataSourceLog_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);










GO
CREATE NONCLUSTERED INDEX [IX_DataSourceLog_Severity]
    ON [integration].[DataSourceLog]([Severity] ASC);


GO
CREATE CLUSTERED INDEX [IX_DataSourceLog_DataSourceTime]
    ON [integration].[DataSourceLog]([DataSourceId] ASC, [LogEntryTime] ASC);

