﻿CREATE TABLE [Retail_BE].[StatusDefinition] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_StatusDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_StatusDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_StatusDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

