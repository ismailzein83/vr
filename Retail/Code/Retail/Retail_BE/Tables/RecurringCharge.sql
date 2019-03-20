CREATE TABLE [Retail_BE].[RecurringCharge] (
    [ID]                    BIGINT          IDENTITY (1, 1) NOT NULL,
    [Amount]                DECIMAL (22, 6) NULL,
    [CurrencyId]            INT             NULL,
    [BED]                   DATETIME        NULL,
    [EED]                   DATETIME        NULL,
    [CreatedTime]           DATETIME        NULL,
    [LastModifiedTime]      DATETIME        NULL,
    [CreatedBy]             INT             NULL,
    [LastModifiedBy]        INT             NULL,
    [Classification]        NVARCHAR (40)   NULL,
    [timestamp]             ROWVERSION      NULL,
    [FinancialAccountId]    NVARCHAR (40)   NULL,
    [RecurringChargeTypeId] BIGINT          NULL,
    [DuePeriod]             INT             NULL,
    [RecurringChargePeriod] NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK__Customer__3214EC273138400F] PRIMARY KEY CLUSTERED ([ID] ASC)
);

