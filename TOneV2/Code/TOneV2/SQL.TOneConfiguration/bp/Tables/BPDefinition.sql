﻿CREATE TABLE [bp].[BPDefinition] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (255)  NOT NULL,
    [Title]       NVARCHAR (255) NOT NULL,
    [FQTN]        VARCHAR (1000) NOT NULL,
    [Config]      NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_BPDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_BPDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

