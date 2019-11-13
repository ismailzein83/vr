﻿CREATE TABLE [TOneWhS_Case].[SupplierFaultTicket] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [SupplierId]        INT              NULL,
    [SupplierZoneId]    BIGINT           NULL,
    [FromDate]          DATETIME         NULL,
    [ToDate]            DATETIME         NULL,
    [Attempts]          INT              NULL,
    [ASR]               DECIMAL (20, 8)  NULL,
    [ACD]               DECIMAL (20, 8)  NULL,
    [CarrierReference]  NVARCHAR (255)   NULL,
    [Notes]             NVARCHAR (1000)  NULL,
    [StatusId]          UNIQUEIDENTIFIER NULL,
    [Attachments]       NVARCHAR (MAX)   NULL,
    [ContactName]       NVARCHAR (255)   NULL,
    [PhoneNumber]       NVARCHAR (255)   NULL,
    [ContactEmails]     NVARCHAR (1000)  NULL,
    [WorkGroupId]       UNIQUEIDENTIFIER NULL,
    [EscalationLevelId] UNIQUEIDENTIFIER NULL,
    [CreatedBy]         INT              NULL,
    [CreatedTime]       DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [TicketDetails]     NVARCHAR (MAX)   NULL,
    [SendEmail]         BIT              NULL,
    [SystemReference]   NVARCHAR (1000)  NULL,
    [AccountManager]    NVARCHAR (255)   NULL,
    [WithAttachments]   BIT              NULL,
    CONSTRAINT [PK_SupplierCase] PRIMARY KEY CLUSTERED ([ID] ASC)
);





