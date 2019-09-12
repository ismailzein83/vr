﻿CREATE TABLE [queue].[QueueActivatorConfig] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             VARCHAR (255)    NOT NULL,
    [Details]          NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_QueueActivatorConfig_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_QueueActivatorConfig_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_QueueActivatorConfig] PRIMARY KEY CLUSTERED ([ID] ASC)
);








GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QueueActivatorConfig_Name]
    ON [queue].[QueueActivatorConfig]([Name] ASC);

