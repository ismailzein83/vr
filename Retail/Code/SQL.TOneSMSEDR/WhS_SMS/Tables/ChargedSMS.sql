CREATE TABLE [WhS_SMS].[ChargedSMS] (
    [ID]                         BIGINT          NOT NULL,
    [Message]                    NVARCHAR (MAX)  NULL,
    [ProxiedMessage]             NVARCHAR (MAX)  NULL,
    [Attempt]                    INT             NULL,
    [ClientRequestDate]          DATETIME        NULL,
    [CustomerId]                 BIGINT          NULL,
    [CustomerAmount]             DECIMAL (20, 8) NULL,
    [DeliveryDate]               DATETIME        NULL,
    [ReplyDate]                  DATETIME        NULL,
    [VendorId]                   BIGINT          NULL,
    [VendorAmount]               DECIMAL (20, 8) NULL,
    [CustomerFinancialAccountId] BIGINT          NULL,
    [SupplierFinancialAccountId] BIGINT          NULL,
    [SupplierCurrencyId]         INT             NULL,
    [CustomerCurrencyId]         INT             NULL,
    [CustomerBillingAccountId]   VARCHAR (50)    NULL,
    [SupplierBillingAccountId]   VARCHAR (50)    NULL,
    [DSTAddressOUT]              VARCHAR (255)   NULL,
    [DestinationZoneId]          BIGINT          NULL,
    CONSTRAINT [PK_ChargedSMS] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_Attempt_ChargedSMS]
    ON [WhS_SMS].[ChargedSMS]([Attempt] ASC);

