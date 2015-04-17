CREATE TABLE [dbo].[Emails] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [SenderName]       VARCHAR (100)  NULL,
    [SenderMail]       NVARCHAR (100) NOT NULL,
    [DestinationName]  VARCHAR (100)  NULL,
    [DestinationEmail] VARCHAR (100)  NOT NULL,
    [CC]               VARCHAR (255)  NULL,
    [BCC]              VARCHAR (255)  NULL,
    [Subject]          VARCHAR (255)  NOT NULL,
    [Body]             VARCHAR (MAX)  NOT NULL,
    [EmailFormat]      VARCHAR (50)   CONSTRAINT [DF_Emails_EmailFormat] DEFAULT ('HTML') NOT NULL,
    [CreationDate]     DATETIME2 (0)  CONSTRAINT [DF_Emails_CreationDate] DEFAULT (getdate()) NOT NULL,
    [IsSent]           BIT            CONSTRAINT [DF_Emails_IsSent] DEFAULT ((0)) NOT NULL,
    [SentDate]         DATETIME2 (0)  NULL,
    [EmailTemplateID]  INT            NULL,
    CONSTRAINT [PK_Emails] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Emails_EmailTemplates] FOREIGN KEY ([EmailTemplateID]) REFERENCES [dbo].[EmailTemplates] ([ID])
);

