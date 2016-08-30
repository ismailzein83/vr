﻿CREATE TABLE [common].[MailMessageTemplate] (
    [ID]            UNIQUEIDENTIFIER NOT NULL,
    [Name]          NVARCHAR (255)   NOT NULL,
    [MessageTypeID] UNIQUEIDENTIFIER NOT NULL,
    [Settings]      NVARCHAR (MAX)   NULL,
    [CreatedTime]   DATETIME         CONSTRAINT [DF_MailMessageTemplate_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]     ROWVERSION       NULL,
    CONSTRAINT [PK_MailMessageTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);



