﻿CREATE TABLE [sec].[Group] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [PSIdentifier]     VARCHAR (50)     NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Description]      NVARCHAR (MAX)   NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_Group_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_Group_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Role_2] PRIMARY KEY CLUSTERED ([ID] ASC)
);













