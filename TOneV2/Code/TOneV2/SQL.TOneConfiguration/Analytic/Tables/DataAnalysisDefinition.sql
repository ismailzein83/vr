﻿CREATE TABLE [Analytic].[DataAnalysisDefinition] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_DataAnalysisDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_DataAnalysisDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_DataAnalysisDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);





