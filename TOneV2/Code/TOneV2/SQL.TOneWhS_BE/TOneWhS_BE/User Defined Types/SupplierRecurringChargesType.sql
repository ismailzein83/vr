CREATE TYPE [TOneWhS_BE].[SupplierRecurringChargesType] AS TABLE (
    [ID]                    BIGINT          NULL,
    [RecurringChargeTypeId] BIGINT          NULL,
    [Amount]                DECIMAL (22, 6) NULL,
    [FinancialAccountId]    BIGINT          NULL,
    [CurrencyId]            INT             NULL,
    [BED]                   DATETIME        NULL,
    [EED]                   DATETIME        NULL,
    [CreatedTime]           DATETIME        NULL,
    [LastModifiedTime]      DATETIME        NULL,
    [CreatedBy]             INT             NULL,
    [LastModifiedBy]        INT             NULL,
    [RecurringChargePeriod] NVARCHAR (MAX)  NULL);



