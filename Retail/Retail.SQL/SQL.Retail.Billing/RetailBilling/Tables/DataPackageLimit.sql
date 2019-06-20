CREATE TABLE [RetailBilling].[DataPackageLimit] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Limit]            INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

