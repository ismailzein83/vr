﻿CREATE TABLE [genericdata].[SummaryTransformationDefinition] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (255)  NOT NULL,
    [Details]     NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_SummaryTransformationDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_SummaryTransformationDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

