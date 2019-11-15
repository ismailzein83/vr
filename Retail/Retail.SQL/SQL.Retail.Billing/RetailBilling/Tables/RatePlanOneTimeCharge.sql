CREATE TABLE [RetailBilling].[RatePlanOneTimeCharge] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [RatePlan]         INT              NULL,
    [ChargeType]       UNIQUEIDENTIFIER NULL,
    [Charge]           NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

