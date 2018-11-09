CREATE TYPE [Retail_Ticket].[FaultTicketType] AS TABLE (
    [ID]               BIGINT           NULL,
    [SubscriberId]     BIGINT           NULL,
    [FromDate]         DATETIME         NULL,
    [ToDate]           DATETIME         NULL,
    [SystemReference]  NVARCHAR (1000)  NULL,
    [Notes]            NVARCHAR (1000)  NULL,
    [SendEmail]        BIT              NULL,
    [PhoneNumber]      NVARCHAR (255)   NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [ContactEmails]    NVARCHAR (1000)  NULL,
    [Attachments]      NVARCHAR (MAX)   NULL,
    [TicketDetails]    NVARCHAR (MAX)   NULL,
    [StatusId]         UNIQUEIDENTIFIER NULL,
    [SourceId]         INT              NULL);

