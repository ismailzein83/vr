﻿CREATE TABLE [genericdata].[GenericRuleDefinition] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (900)   NOT NULL,
    [Details]          NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_GenericRuleDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_GenericRuleDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_GenericRuleDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);









