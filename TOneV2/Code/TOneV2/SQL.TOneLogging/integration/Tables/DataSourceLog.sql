﻿CREATE TABLE [integration].[DataSourceLog] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [DataSourceId]    INT            NOT NULL,
    [Severity]        INT            NOT NULL,
    [Message]         VARCHAR (1000) NULL,
    [ImportedBatchId] INT            NULL,
    [LogEntryTime]    DATETIME       NULL,
    CONSTRAINT [PK_DataSourceLog] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_DataSourceLog_DataSourceImportedBatch] FOREIGN KEY ([ImportedBatchId]) REFERENCES [integration].[DataSourceImportedBatch] ([ID])
);

