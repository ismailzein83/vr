﻿CREATE TABLE [integration].[DataSource] (
    [ID]           UNIQUEIDENTIFIER CONSTRAINT [DF__DataSource__ID__485B9C89] DEFAULT (newid()) NOT NULL,
    [Name]         VARCHAR (100)    NOT NULL,
    [AdapterID]    UNIQUEIDENTIFIER NULL,
    [AdapterState] VARCHAR (1000)   NULL,
    [TaskId]       UNIQUEIDENTIFIER NOT NULL,
    [Settings]     VARCHAR (MAX)    NOT NULL,
	[LastModifiedTime] [datetime]	NULL CONSTRAINT [DF_DataSource_LastModifiedTime]  DEFAULT (getdate()),
    [CreatedTime]  DATETIME         CONSTRAINT [DF_DataSource_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]    ROWVERSION       NULL,
    CONSTRAINT [pk_DataSource] PRIMARY KEY CLUSTERED ([ID] ASC)
);







