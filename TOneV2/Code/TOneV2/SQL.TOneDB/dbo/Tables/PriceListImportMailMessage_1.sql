CREATE TABLE [dbo].[PriceListImportMailMessage] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [MessageID]          INT            NOT NULL,
    [Subject]            VARCHAR (1024) NULL,
    [From]               VARCHAR (1024) NULL,
    [To]                 VARCHAR (1024) NULL,
    [Body]               VARCHAR (MAX)  NULL,
    [DeliveryDate]       DATETIME       NULL,
    [CC]                 VARCHAR (MAX)  NULL,
    [BCC]                VARCHAR (MAX)  NULL,
    [AttachmentFileName] VARCHAR (255)  NULL,
    [Attachment]         IMAGE          NULL,
    [ContentType]        VARCHAR (50)   NULL,
    [ContentDisposition] VARCHAR (50)   NULL,
    [Processed]          BIT            NULL,
    [timestamp]          ROWVERSION     NOT NULL,
    CONSTRAINT [PK_MailMessage] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_PriceListImportMailMessage]
    ON [dbo].[PriceListImportMailMessage]([MessageID] ASC);

