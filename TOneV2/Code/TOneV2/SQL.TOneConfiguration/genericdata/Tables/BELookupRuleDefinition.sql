﻿CREATE TABLE [genericdata].[BELookupRuleDefinition] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (450) NOT NULL,
    [Details]     NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_BELookupRuleDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_BELookupRuleDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_BELookupRuleDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

