CREATE TABLE [RetailBilling].[RatePlanServicePackage] (
    [ID]               INT             IDENTITY (1, 1) NOT NULL,
    [RatePlanID]       INT             NULL,
    [Name]             NVARCHAR (255)  NULL,
    [ActivationFee]    DECIMAL (20, 8) NULL,
    [RecurringFee]     DECIMAL (20, 8) NULL,
    [RecurringPeriod]  INT             NULL,
    [CreatedTime]      DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [timestamp]        ROWVERSION      NULL,
    CONSTRAINT [PK__RatePlan__3214EC27164452B1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

