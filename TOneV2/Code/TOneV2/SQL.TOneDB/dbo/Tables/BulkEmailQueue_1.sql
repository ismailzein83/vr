CREATE TABLE [dbo].[BulkEmailQueue] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [BatchID]      INT           NOT NULL,
    [MailTemplate] VARCHAR (50)  NULL,
    [FromEmails]   VARCHAR (500) NULL,
    [ToEmails]     VARCHAR (500) NULL,
    [Body]         VARCHAR (MAX) NULL,
    [Subject]      VARCHAR (500) NULL,
    [CCEmail]      VARCHAR (500) NULL,
    [BCCEmail]     VARCHAR (500) NULL,
    [IsSent]       BIT           NULL,
    [timestamp]    ROWVERSION    NULL,
    CONSTRAINT [PK_BulkMailQueue] PRIMARY KEY CLUSTERED ([ID] ASC)
);

