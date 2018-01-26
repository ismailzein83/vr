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
    [StatusId]         UNIQUEIDENTIFIER NULL);

