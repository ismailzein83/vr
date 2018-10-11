﻿CREATE TYPE [TOneWhS_Case].[CustomerFaultTicketType] AS TABLE (
    [ID]                BIGINT           NULL,
    [CustomerId]        INT              NULL,
    [SaleZoneId]        BIGINT           NULL,
    [FromDate]          DATETIME         NULL,
    [ToDate]            DATETIME         NULL,
    [Attempts]          INT              NULL,
    [ASR]               DECIMAL (20, 8)  NULL,
    [ACD]               DECIMAL (20, 8)  NULL,
    [CarrierReference]  NVARCHAR (255)   NULL,
    [StatusId]          UNIQUEIDENTIFIER NULL,
    [Notes]             NVARCHAR (1000)  NULL,
    [SystemReference]   NVARCHAR (1000)  NULL,
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
    [AccountManager]    NVARCHAR (255)   NULL);



