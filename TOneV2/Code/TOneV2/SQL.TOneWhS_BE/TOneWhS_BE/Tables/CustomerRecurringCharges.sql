CREATE TABLE [TOneWhS_BE].[CustomerRecurringCharges] (
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
    [FinancialAccountId]    BIGINT          NULL,
    [RecurringChargePeriod] NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_CustomerRecurringCharges] PRIMARY KEY CLUSTERED ([ID] ASC)
);

