CREATE TABLE [VR_Invoice].[PartnerInvoiceSetting] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [PartnerID]        VARCHAR (50)     NOT NULL,
    [InvoiceSettingID] UNIQUEIDENTIFIER NOT NULL,
    [Details]          NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_PartnerInvoiceSetting_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_PartnerInvoiceSetting] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PartnerInvoiceSetting]
    ON [VR_Invoice].[PartnerInvoiceSetting]([InvoiceSettingID] ASC, [PartnerID] ASC);

