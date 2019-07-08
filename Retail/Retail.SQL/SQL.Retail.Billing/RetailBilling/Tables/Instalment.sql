CREATE TABLE [RetailBilling].[Instalment] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [Value]            DECIMAL (20, 8) NULL,
    [Date]             DATETIME        NULL,
    [CreatedTime]      DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [BillingAccountId] BIGINT          NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

