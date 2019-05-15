﻿CREATE TABLE [genericdata].[DataTransformationDefinition] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (900)   NOT NULL,
    [Title]            NVARCHAR (1000)  NOT NULL,
    [Details]          NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_DataTransformationDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_DataTransformationDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_DataTransformationDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);











