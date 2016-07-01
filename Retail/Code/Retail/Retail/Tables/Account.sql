﻿CREATE TABLE [Retail].[Account] (
    [ID]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (255)   NOT NULL,
    [TypeID]              INT              NOT NULL,
    [Settings]            NVARCHAR (MAX)   NULL,
    [ParentID]            INT              NULL,
    [StatusID]            UNIQUEIDENTIFIER NULL,
    [ExecutedActionsData] NVARCHAR (MAX)   NULL,
    [CreatedTime]         DATETIME         CONSTRAINT [DF_Account_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]           ROWVERSION       NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([ID] ASC)
);





