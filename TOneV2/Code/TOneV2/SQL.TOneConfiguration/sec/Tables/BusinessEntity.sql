﻿CREATE TABLE [sec].[BusinessEntity] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]      UNIQUEIDENTIFIER NULL,
    [Name]              NVARCHAR (255)   NOT NULL,
    [Title]             NVARCHAR (255)   NOT NULL,
    [ModuleId]          UNIQUEIDENTIFIER NOT NULL,
    [BreakInheritance]  BIT              NOT NULL,
    [PermissionOptions] NVARCHAR (255)   NOT NULL,
    [timestamp]         ROWVERSION       NOT NULL,
    [LastModifiedTime]  DATETIME         CONSTRAINT [DF_BusinessEntity_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BusinessEntity_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_BusinessEntity_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



















