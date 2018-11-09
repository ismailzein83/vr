CREATE TABLE [Retail_Ticket].[FaultTicket] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [SubscriberId]     BIGINT           NULL,
    [FromDate]         DATETIME         NULL,
    [ToDate]           DATETIME         NULL,
    [SystemReference]  NVARCHAR (1000)  NULL,
    [Notes]            NVARCHAR (1000)  NULL,
    [SendEmail]        BIT              NULL,
    [PhoneNumber]      NVARCHAR (255)   NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_FaultTicket_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [ContactEmails]    NVARCHAR (1000)  NULL,
    [Attachments]      NVARCHAR (MAX)   NULL,
    [TicketDetails]    NVARCHAR (MAX)   NULL,
    [StatusId]         UNIQUEIDENTIFIER NULL,
    [SourceId]         INT              NULL,
    CONSTRAINT [PK_FaultTicket] PRIMARY KEY CLUSTERED ([ID] ASC)
);

