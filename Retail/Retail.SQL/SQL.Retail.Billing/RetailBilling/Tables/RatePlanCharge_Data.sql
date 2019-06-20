CREATE TABLE [RetailBilling].[RatePlanCharge_Data] (
    [ID]                        INT              IDENTITY (1, 1) NOT NULL,
    [RatePlanID]                INT              NULL,
    [SpeedFrom]                 INT              NULL,
    [SpeedTo]                   INT              NULL,
    [SpeedType]                 INT              NULL,
    [PackageLimit]              UNIQUEIDENTIFIER NULL,
    [ActivationFee]             DECIMAL (20, 8)  NULL,
    [RecurringFee]              DECIMAL (20, 8)  NULL,
    [RecurringFeeNbOfMbps]      INT              NULL,
    [BankGuarantee]             DECIMAL (20, 8)  NULL,
    [MultiplyGuaranteeByE1]     BIT              NULL,
    [AdvancedPaymentNbOfMonths] INT              NULL,
    [TechnologyID]              UNIQUEIDENTIFIER NULL,
    [ServiceTypeID]             UNIQUEIDENTIFIER NULL,
    [CreatedTime]               DATETIME         NULL,
    [CreatedBy]                 INT              NULL,
    [LastModifiedTime]          DATETIME         NULL,
    [LastModifiedBy]            INT              NULL,
    [timestamp]                 ROWVERSION       NULL,
    CONSTRAINT [PK__RatePlan__3214EC2733D4B598] PRIMARY KEY CLUSTERED ([ID] ASC)
);

