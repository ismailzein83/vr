CREATE TYPE [TOneWhS_Case].[SupplierCaseType] AS TABLE (
    [ID]               BIGINT           NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CaseTime]         DATETIME         NULL,
    [SupplierId]       INT              NULL,
    [SupplierZoneId]   BIGINT           NULL,
    [FromDate]         DATETIME         NULL,
    [ToDate]           DATETIME         NULL,
    [Attempts]         INT              NULL,
    [ASR]              DECIMAL (20, 8)  NULL,
    [ACD]              DECIMAL (20, 8)  NULL,
    [CarrierReference] NVARCHAR (255)   NULL,
    [Description]      NVARCHAR (1000)  NULL,
    [Notes]            NVARCHAR (1000)  NULL,
    [StatusId]         UNIQUEIDENTIFIER NULL,
    [Attachments]      NVARCHAR (MAX)   NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [ContactName]      NVARCHAR (255)   NULL,
    [PhoneNumber]      NVARCHAR (255)   NULL,
    [ContactEmails]    NVARCHAR (1000)  NULL);



