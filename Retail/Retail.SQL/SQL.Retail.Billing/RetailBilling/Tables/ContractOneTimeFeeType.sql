CREATE TABLE [RetailBilling].[ContractOneTimeFeeType] (
    [ID]                          UNIQUEIDENTIFIER NOT NULL,
    [Name]                        NVARCHAR (255)   NULL,
    [ContractTypeID]              UNIQUEIDENTIFIER NULL,
    [ChargeTypeID]                UNIQUEIDENTIFIER NULL,
    [AdvancedPaymentChargeTypeID] UNIQUEIDENTIFIER NULL,
    [DepositChargeTypeID]         UNIQUEIDENTIFIER NULL,
    [CreatedTime]                 DATETIME         NULL,
    [CreatedBy]                   INT              NULL,
    [LastModifiedTime]            DATETIME         NULL,
    [LastModifiedBy]              INT              NULL,
    [timestamp]                   ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

