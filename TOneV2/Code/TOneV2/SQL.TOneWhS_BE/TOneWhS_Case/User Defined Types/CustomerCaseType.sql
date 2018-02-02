CREATE TYPE [TOneWhS_Case].[CustomerCaseType] AS TABLE (
    [ID]                BIGINT           NULL,
    [CaseTime]          DATETIME         NULL,
    [CustomerId]        INT              NULL,
    [SaleZoneId]        BIGINT           NULL,
    [FromDate]          DATETIME         NULL,
    [ToDate]            DATETIME         NULL,
    [Attempts]          INT              NULL,
    [ASR]               DECIMAL (20, 8)  NULL,
    [ACD]               DECIMAL (20, 8)  NULL,
    [CarrierReference]  NVARCHAR (255)   NULL,
    [Description]       NVARCHAR (1000)  NULL,
    [StatusId]          UNIQUEIDENTIFIER NULL,
    [Notes]             NVARCHAR (1000)  NULL,
    [OwnerReference]    NVARCHAR (1000)  NULL,
    [Attachments]       NVARCHAR (MAX)   NULL,
    [WorkGroupId]       UNIQUEIDENTIFIER NULL,
    [Settings]          NVARCHAR (MAX)   NULL,
    [ContactName]       NVARCHAR (255)   NULL,
    [PhoneNumber]       NVARCHAR (255)   NULL,
    [ContactEmails]     NVARCHAR (1000)  NULL,
    [EscalationLevelId] UNIQUEIDENTIFIER NULL);





