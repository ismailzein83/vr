CREATE TABLE [RetailBilling].[RatePlanServicePackageService] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [ServicePackageID] INT              NULL,
    [ServiceID]        UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__RatePlan__3214EC271BFD2C07] PRIMARY KEY CLUSTERED ([ID] ASC)
);

