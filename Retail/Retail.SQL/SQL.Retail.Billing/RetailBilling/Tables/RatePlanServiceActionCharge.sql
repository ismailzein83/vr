CREATE TABLE [RetailBilling].[RatePlanServiceActionCharge] (
    [ID]                 INT              IDENTITY (1, 1) NOT NULL,
    [RatePlanID]         INT              NULL,
    [ContractTypeID]     UNIQUEIDENTIFIER NULL,
    [ActionChargeTypeID] UNIQUEIDENTIFIER NULL,
    [Charge]             NVARCHAR (MAX)   NULL,
    [AdvancedPayment]    NVARCHAR (MAX)   NULL,
    [Deposit]            NVARCHAR (MAX)   NULL,
    [CreatedTime]        DATETIME         NULL,
    [CreatedBy]          INT              NULL,
    [LastModifiedTime]   DATETIME         NULL,
    [LastModifiedBy]     INT              NULL,
    [timestamp]          ROWVERSION       NULL,
    [CurrencyID]         INT              NULL,
    CONSTRAINT [PK__RatePlan__3214EC2770A8B9AE] PRIMARY KEY CLUSTERED ([ID] ASC)
);

