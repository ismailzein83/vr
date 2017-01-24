CREATE TABLE [VR_Invoice].[InvoiceSetting] (
    [ID]            UNIQUEIDENTIFIER NOT NULL,
    [InvoiceTypeId] UNIQUEIDENTIFIER NULL,
    [Name]          VARCHAR (50)     NULL,
    [IsDefault]     BIT              NULL,
    [Details]       NVARCHAR (MAX)   NULL,
    [CreatedTime]   DATETIME         CONSTRAINT [DF_InvoiceSetting_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]     ROWVERSION       NULL,
    CONSTRAINT [PK_InvoiceSetting] PRIMARY KEY CLUSTERED ([ID] ASC)
);

