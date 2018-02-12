CREATE TABLE [TOneWhS_Case].[CustomerCase] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [OwnerReference]    NVARCHAR (1000)  NOT NULL,
    [FromDate]          DATETIME         NULL,
    [ToDate]            DATETIME         NULL,
    [Attempts]          INT              NULL,
    [ASR]               DECIMAL (20, 8)  NULL,
    [ACD]               DECIMAL (20, 8)  NULL,
    [CarrierReference]  NVARCHAR (255)   NULL,
    [SaleZoneId]        BIGINT           NULL,
    [CustomerId]        INT              NULL,
    [StatusId]          UNIQUEIDENTIFIER NULL,
    [Notes]             NVARCHAR (1000)  NULL,
    [Attachments]       NVARCHAR (MAX)   NULL,
    [WorkGroupId]       UNIQUEIDENTIFIER NULL,
    [ContactName]       NVARCHAR (255)   NULL,
    [PhoneNumber]       NVARCHAR (255)   NULL,
    [ContactEmails]     NVARCHAR (1000)  NULL,
    [EscalationLevelId] UNIQUEIDENTIFIER NULL,
    [CreatedBy]         INT              NULL,
    [CreatedTime]       DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [TicketDetails]     NVARCHAR (MAX)   NULL,
    [SendEmail]         BIT              NULL,
    CONSTRAINT [PK_CustomerCase] PRIMARY KEY CLUSTERED ([ID] ASC)
);









