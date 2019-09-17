﻿CREATE TABLE [queue].[ExecutionFlowDefinition] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF__ExecutionFlo__Id__1C7D1A4B] DEFAULT (newid()) NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             VARCHAR (255)    NOT NULL,
    [Title]            NVARCHAR (255)   NOT NULL,
    [ExecutionTree]    NVARCHAR (MAX)   NULL,
    [Stages]           NVARCHAR (MAX)   NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_ExecutionFlowDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [pk_ExecutionFlowDefinition] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_ExecutionFlowType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);













