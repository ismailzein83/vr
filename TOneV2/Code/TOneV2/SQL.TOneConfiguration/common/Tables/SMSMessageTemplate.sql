CREATE TABLE [common].[SMSMessageTemplate] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (255)    NULL,
    [SMSMessageTypeId] UNIQUEIDENTIFIER NULL,
    [Settings]         VARCHAR (MAX)    NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_SMSMessageTemplate_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        NCHAR (10)       NULL,
    CONSTRAINT [PK_SMSMessageTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);

