CREATE TYPE [VR_Invoice].[InvoiceReportFileType] AS TABLE (
    [ID]               UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NULL,
    [ReportName]       NVARCHAR (255)   NULL,
    [InvoiceTypeId]    UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL);

