﻿CREATE TABLE [common].[ExtensionConfiguration] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Title]            NVARCHAR (255)   NULL,
    [ConfigType]       NVARCHAR (255)   NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_ExtensionConfiguration_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_ExtensionConfiguration_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ExtensionConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC)
);
























GO


