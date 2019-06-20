CREATE TABLE [RetailBilling].[RatePlanItemCharge] (
    [ID]                      INT              IDENTITY (1, 1) NOT NULL,
    [RatePlanID]              INT              NULL,
    [ContractSubTypeID]       UNIQUEIDENTIFIER NULL,
    [ActivationCharge]        NVARCHAR (MAX)   NULL,
    [RecurringCharge]         NVARCHAR (MAX)   NULL,
    [SuspensionCharge]        NVARCHAR (MAX)   NULL,
    [DeactivationCharge]      NVARCHAR (MAX)   NULL,
    [DepositCharge]           NVARCHAR (MAX)   NULL,
    [BankGuaranteeCharge]     NVARCHAR (MAX)   NULL,
    [AdvancedPaymentNbMonths] INT              NULL,
    [CreatedTime]             DATETIME         NULL,
    [CreatedBy]               INT              NULL,
    [LastModifiedTime]        DATETIME         NULL,
    [LastModifiedBy]          INT              NULL,
    [timestamp]               ROWVERSION       NULL
);

