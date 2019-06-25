CREATE TABLE [RetailBilling].[Deposit] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [ContractId]       BIGINT          NULL,
    [ServiceId]        BIGINT          NULL,
    [CurrencyId]       INT             NULL,
    [DepositAmount]    DECIMAL (20, 8) NULL,
    [CreatedTime]      DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    CONSTRAINT [PK__Deposit__3214EC2718EBB532] PRIMARY KEY CLUSTERED ([ID] ASC)
);

