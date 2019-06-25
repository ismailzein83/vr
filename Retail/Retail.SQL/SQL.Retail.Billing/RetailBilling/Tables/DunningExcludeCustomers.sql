CREATE TABLE [RetailBilling].[DunningExcludeCustomers] (
    [ID]               BIGINT   IDENTITY (1, 1) NOT NULL,
    [FromDate]         DATETIME NULL,
    [ToDate]           DATETIME NULL,
    [CreatedTime]      DATETIME NULL,
    [CreatedBy]        INT      NULL,
    [LastModifiedTime] DATETIME NULL,
    [LastModifiedBy]   INT      NULL,
    [CustomerId]       BIGINT   NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

