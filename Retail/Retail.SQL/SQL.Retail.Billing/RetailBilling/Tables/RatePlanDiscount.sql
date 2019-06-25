CREATE TABLE [RetailBilling].[RatePlanDiscount] (
    [ID]                  INT              IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (255)   NULL,
    [CustomerType]        UNIQUEIDENTIFIER NULL,
    [CustomerCategory]    UNIQUEIDENTIFIER NULL,
    [CustomerSubCategory] UNIQUEIDENTIFIER NULL,
    [Service]             UNIQUEIDENTIFIER NULL,
    [DiscountPercentage]  DECIMAL (20, 8)  NULL,
    [CreatedTime]         DATETIME         NULL,
    [CreatedBy]           INT              NULL,
    [LastModifiedTime]    DATETIME         NULL,
    [LastModifiedBy]      INT              NULL,
    [RatePlanID]          INT              NULL
);

