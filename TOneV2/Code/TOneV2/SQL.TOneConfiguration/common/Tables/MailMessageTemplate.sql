﻿CREATE TABLE [common].[MailMessageTemplate] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [MessageTypeID]    UNIQUEIDENTIFIER NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_MailMessageTemplate_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_MailMessageTemplate_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_MailMessageTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);







