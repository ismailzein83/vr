﻿CREATE TABLE [common].[Connection] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_Connection_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [timestamp]        ROWVERSION       NOT NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_Connection_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_common.Connection] PRIMARY KEY CLUSTERED ([ID] ASC)
);





