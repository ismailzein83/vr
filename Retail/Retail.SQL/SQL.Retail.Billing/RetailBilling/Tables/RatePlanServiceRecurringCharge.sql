CREATE TABLE [RetailBilling].[RatePlanServiceRecurringCharge] (
    [ID]                    INT              IDENTITY (1, 1) NOT NULL,
    [RatePlanID]            INT              NULL,
    [RecurringChargeTypeID] UNIQUEIDENTIFIER NULL,
    [Charge]                NVARCHAR (MAX)   NULL,
    [RecurringPeriod]       INT              NULL,
    [ProrateOnStart]        BIT              NULL,
    [ProrateOnEnd]          BIT              NULL,
    [CreatedTime]           DATETIME         NULL,
    [CreatedBy]             INT              NULL,
    [LastModifiedTime]      DATETIME         NULL,
    [LastModifiedBy]        INT              NULL,
    [timestamp]             ROWVERSION       NULL,
    [CurrencyID]            INT              NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

