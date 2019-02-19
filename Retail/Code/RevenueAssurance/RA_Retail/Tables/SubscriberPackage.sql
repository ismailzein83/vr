CREATE TABLE [RA_Retail].[SubscriberPackage] (
    [ID]               BIGINT   IDENTITY (1, 1) NOT NULL,
    [SubscriberId]     BIGINT   NULL,
    [PackageId]        INT      NULL,
    [BED]              DATETIME NULL,
    [CreatedTime]      DATETIME NULL,
    [EED]              DATETIME NULL,
    [CreatedBy]        INT      NULL,
    [LastModifiedTime] DATETIME NULL,
    [LastModifiedBy]   INT      NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

