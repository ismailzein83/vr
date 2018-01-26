CREATE TYPE [TOneWhS_Case].[CustomerCaseType] AS TABLE (
    [ID]               BIGINT           NULL,
    [CaseTime]         DATETIME         NULL,
    [CustomerId]       INT              NULL,
    [SaleZoneId]       BIGINT           NULL,
    [FromDate]         DATETIME         NULL,
    [ToDate]           DATETIME         NULL,
    [Attempts]         INT              NULL,
    [ASR]              DECIMAL (20, 8)  NULL,
    [ACD]              DECIMAL (20, 8)  NULL,
    [CarrierReference] NVARCHAR (255)   NULL,
    [Description]      NVARCHAR (1000)  NULL,
    [StatusId]         UNIQUEIDENTIFIER NULL,
    [Notes]            NVARCHAR (1000)  NULL,
    [ReasonId]         UNIQUEIDENTIFIER NULL,
    [OwnerReference]   NVARCHAR (1000)  NULL,
    [Attachments]      NVARCHAR (MAX)   NULL,
    [WorkGroupId]      UNIQUEIDENTIFIER NULL);

