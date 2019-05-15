﻿CREATE TABLE [common].[VRObjectTypeDefinition] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRObjectTypeDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_VRObjectTypeDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_VRObjectTypeDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_VRObjectTypeDefinition] UNIQUE NONCLUSTERED ([Name] ASC)
);









