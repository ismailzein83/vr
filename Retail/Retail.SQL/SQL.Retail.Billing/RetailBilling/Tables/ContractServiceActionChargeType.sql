CREATE TABLE [RetailBilling].[ContractServiceActionChargeType] (
    [ID]                          UNIQUEIDENTIFIER NOT NULL,
    [ContractTypeID]              UNIQUEIDENTIFIER NULL,
    [ContractServiceTypeID]       UNIQUEIDENTIFIER NULL,
    [ContractServiceTypeOptionID] UNIQUEIDENTIFIER NULL,
    [ActionTypeID]                UNIQUEIDENTIFIER NULL,
    [ChargeTypeID]                UNIQUEIDENTIFIER NULL,
    [AdvancedPaymentChargeTypeID] UNIQUEIDENTIFIER NULL,
    [DepositChargeTypeID]         UNIQUEIDENTIFIER NULL,
    [CreatedTime]                 DATETIME         NULL,
    [CreatedBy]                   INT              NULL,
    [LastModifiedTime]            DATETIME         NULL,
    [LastModifiedBy]              INT              NULL,
    [timestamp]                   ROWVERSION       NULL,
    CONSTRAINT [PK__Contract__3214EC275F7E2DAC] PRIMARY KEY CLUSTERED ([ID] ASC)
);

