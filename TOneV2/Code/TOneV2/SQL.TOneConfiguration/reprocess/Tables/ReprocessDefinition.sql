﻿CREATE TABLE [reprocess].[ReprocessDefinition] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF__ReprocessDef__Id__16C440F5] DEFAULT (newid()) NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_ReprocessDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_ReprocessDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [pk_ReprocessDefinition] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_ReprocessDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);













