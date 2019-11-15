CREATE TABLE [RetailBilling].[DiscountRule] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [TargetType]       UNIQUEIDENTIFIER NULL,
    [Condition]        NVARCHAR (MAX)   NULL,
    [BED]              DATETIME         NULL,
    [EED]              DATETIME         NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedTime] DATETIME         NULL,
    [Percentage]       DECIMAL (20, 8)  NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

