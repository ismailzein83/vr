﻿CREATE TABLE [genericdata].[DataStore] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (1000)  NOT NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_DataStore_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_DataStore_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_DataStore] PRIMARY KEY CLUSTERED ([ID] ASC)
);









