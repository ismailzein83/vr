﻿CREATE TABLE [sec].[BusinessEntityModule] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [OldId]            INT              IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [ParentId]         UNIQUEIDENTIFIER NULL,
    [OldParentId]      INT              NULL,
    [BreakInheritance] BIT              NOT NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_BusinessEntityModule_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);











