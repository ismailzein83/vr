CREATE TABLE [TOneWhS_Invoice].[CarrierInvoiceAccount] (
    [ID]                     INT            IDENTITY (1, 1) NOT NULL,
    [CarrierProfileId]       INT            NULL,
    [CarrierAccountId]       INT            NULL,
    [InvoiceAccountSettings] NVARCHAR (MAX) NULL,
    [BED]                    DATETIME       NOT NULL,
    [EED]                    DATETIME       NULL,
    [CreatedTime]            DATETIME       CONSTRAINT [DF_CarrierInvoiceAccount_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]              ROWVERSION     NULL,
    CONSTRAINT [PK_CarrierInvoiceAccount] PRIMARY KEY CLUSTERED ([ID] ASC)
);

