CREATE TABLE [RetailBilling].[ContractServiceRecurringChargeType] (
    [ID]                          UNIQUEIDENTIFIER NOT NULL,
    [ContractServiceTypeID]       UNIQUEIDENTIFIER NULL,
    [ContractServiceTypeOptionID] UNIQUEIDENTIFIER NULL,
    [ChargeableConditionID]       UNIQUEIDENTIFIER NULL,
    [ChargeTypeID]                UNIQUEIDENTIFIER NULL,
    [CreatedTime]                 DATETIME         NULL,
    [CreatedBy]                   INT              NULL,
    [LastModifiedTime]            DATETIME         NULL,
    [LastModifiedBy]              INT              NULL,
    [timestamp]                   ROWVERSION       NULL,
    [DefaultCharge]               NVARCHAR (MAX)   NULL,
    [UnavailableInRatePlan]       BIT              NULL,
    CONSTRAINT [PK__Contract__3214EC2757DD0BE4] PRIMARY KEY CLUSTERED ([ID] ASC)
);



