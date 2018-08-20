CREATE TABLE [VR_Invoice].[InvoiceReportFile] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [ReportName]       NVARCHAR (255)   NULL,
    [InvoiceTypeId]    UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_InvoiceReportFile_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_InvoiceReportFile] PRIMARY KEY CLUSTERED ([ID] ASC)
);

