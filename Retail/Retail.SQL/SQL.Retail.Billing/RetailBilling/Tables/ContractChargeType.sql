CREATE TABLE [RetailBilling].[ContractChargeType] (
    [ID]                     UNIQUEIDENTIFIER NOT NULL,
    [Name]                   NVARCHAR (255)   NULL,
    [ContractTypeID]         UNIQUEIDENTIFIER NULL,
    [IsRecurring]            BIT              NULL,
    [DefaultChargeValue]     NVARCHAR (MAX)   NULL,
    [DefaultRecurringPeriod] INT              NULL,
    [CreatedTime]            DATETIME         NULL,
    [CreatedBy]              INT              NULL,
    [LastModifiedTime]       DATETIME         NULL,
    [LastModifiedBy]         INT              NULL,
    [timestamp]              ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

