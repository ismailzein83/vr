CREATE TABLE [dbo].[FaultTicketUpdate] (
    [FaultTicketUpdateID] INT           IDENTITY (1, 1) NOT NULL,
    [FaultTicketID]       INT           NOT NULL,
    [SendMail]            CHAR (1)      NULL,
    [Email]               VARCHAR (255) NULL,
    [Contact]             VARCHAR (50)  NULL,
    [PhoneNo]             VARCHAR (50)  NULL,
    [UserID]              INT           NULL,
    [Notes]               VARCHAR (MAX) NULL,
    [Status]              TINYINT       NOT NULL,
    [UpdateDate]          DATETIME      NOT NULL,
    [FileName]            VARCHAR (50)  NULL,
    [Attachment]          IMAGE         NULL,
    [timestamp]           ROWVERSION    NOT NULL,
    CONSTRAINT [PK_FaultTicketUpdate] PRIMARY KEY CLUSTERED ([FaultTicketUpdateID] ASC),
    CONSTRAINT [FK_FaultTicketUpdate_FaultTicket] FOREIGN KEY ([FaultTicketID]) REFERENCES [dbo].[FaultTicket] ([FaultTicketID]) ON DELETE CASCADE ON UPDATE CASCADE
);

