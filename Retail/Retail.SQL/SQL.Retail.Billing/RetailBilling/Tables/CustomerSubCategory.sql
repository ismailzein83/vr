CREATE TABLE [RetailBilling].[CustomerSubCategory] (
    [ID]                 UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (255)   NULL,
    [CustomerCategoryID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]        DATETIME         NULL,
    [CreatedBy]          INT              NULL,
    [LastModifiedTime]   DATETIME         NULL,
    [LastModifiedBy]     INT              NULL,
    [timestamp]          ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

