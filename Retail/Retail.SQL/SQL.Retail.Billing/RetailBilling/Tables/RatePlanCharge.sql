CREATE TABLE [RetailBilling].[RatePlanCharge] (
    [ID]                   INT              IDENTITY (1, 1) NOT NULL,
    [RatePlanID]           INT              NULL,
    [ChargeTypeID]         UNIQUEIDENTIFIER NULL,
    [Charge]               NVARCHAR (MAX)   NULL,
    [RecurringPeriod]      INT              NULL,
    [ProrateOnPeriodStart] BIT              NULL,
    [ProrateOnPeriodEnd]   BIT              NULL,
    [CreatedTime]          DATETIME         NULL,
    [CreatedBy]            INT              NULL,
    [LastModifiedTime]     DATETIME         NULL,
    [LastModifiedBy]       INT              NULL,
    [timestamp]            ROWVERSION       NULL,
    CONSTRAINT [PK__RatePlan__3214EC2725869641] PRIMARY KEY CLUSTERED ([ID] ASC)
);



