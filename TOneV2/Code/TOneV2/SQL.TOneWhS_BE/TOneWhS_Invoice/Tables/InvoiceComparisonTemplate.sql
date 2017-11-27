CREATE TABLE [TOneWhS_Invoice].[InvoiceComparisonTemplate] (
    [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [InvoiceTypeId] UNIQUEIDENTIFIER NULL,
    [PartnerId]     NVARCHAR (255)   NULL,
    [Details]       NVARCHAR (MAX)   NULL,
    [CreatedTime]   DATETIME         CONSTRAINT [DF_InvoiceComparisonTemplate_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]     ROWVERSION       NULL,
    CONSTRAINT [PK_InvoiceComparisonTemplate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_InvoiceComparisonTemplate] UNIQUE NONCLUSTERED ([InvoiceTypeId] ASC, [PartnerId] ASC)
);

