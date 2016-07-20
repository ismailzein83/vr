CREATE TABLE [VR_AccountBalance].[BillingTransactionType] (
    [ID]   UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR (255)   NOT NULL,
    CONSTRAINT [PK_BillingTransactionType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_BillingTransactionType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

