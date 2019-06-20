﻿CREATE TABLE [RetailBilling].[RatePlanService] (
    [ID]                                 INT              IDENTITY (1, 1) NOT NULL,
    [RatePlanID]                         INT              NULL,
    [ServiceID]                          UNIQUEIDENTIFIER NULL,
    [ActivationFee]                      NVARCHAR (MAX)   NULL,
    [RecurringFee]                       NVARCHAR (MAX)   NULL,
    [RecurringPeriod]                    INT              NULL,
    [CreatedTime]                        DATETIME         NULL,
    [CreatedBy]                          INT              NULL,
    [LastModifiedTime]                   DATETIME         NULL,
    [LastModifiedBy]                     INT              NULL,
    [timestamp]                          ROWVERSION       NULL,
    [SuspensionCharge]                   NVARCHAR (MAX)   NULL,
    [SuspensionRecurringCharge]          NVARCHAR (MAX)   NULL,
    [SuspensionRecurringPeriod]          INT              NULL,
    [DeactivationCharge]                 NVARCHAR (MAX)   NULL,
    [Deposit]                            NVARCHAR (MAX)   NULL,
    [BankGuarantee]                      NVARCHAR (MAX)   NULL,
    [AdvancedPaymentNbBillingCycles]     INT              NULL,
    [ProratedOnActivation]               BIT              NULL,
    [ProratedOnDeactivation]             BIT              NULL,
    [RecurringChargeInAdvance]           BIT              NULL,
    [SuspensionRecurringChargeInAdvance] BIT              NULL,
    CONSTRAINT [PK__RatePlan__3214EC270AD2A005] PRIMARY KEY CLUSTERED ([ID] ASC)
);

