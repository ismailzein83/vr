CREATE TABLE [TOneWhS_BE].[SupplierRecurringCharges] (
    [ID]                    BIGINT          IDENTITY (1, 1) NOT NULL,
    [RecurringChargeTypeId] BIGINT          NULL,
    [Amount]                DECIMAL (22, 6) NULL,
    [CurrencyId]            INT             NULL,
    [BED]                   DATETIME        NULL,
    [EED]                   DATETIME        NULL,
    [CreatedTime]           DATETIME        NULL,
    [LastModifiedTime]      DATETIME        NULL,
    [CreatedBy]             INT             NULL,
    [LastModifiedBy]        INT             NULL,
    [timestamp]             ROWVERSION      NULL,
    CONSTRAINT [PK_SupplierRecurringCharges] PRIMARY KEY CLUSTERED ([ID] ASC)
);

