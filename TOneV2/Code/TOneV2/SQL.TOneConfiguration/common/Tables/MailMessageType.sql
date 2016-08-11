CREATE TABLE [common].[MailMessageType] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [timestamp]   ROWVERSION       NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_MailMessageType_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_MailMessageType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

