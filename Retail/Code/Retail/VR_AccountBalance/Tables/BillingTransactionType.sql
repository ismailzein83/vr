CREATE TABLE [VR_AccountBalance].[BillingTransactionType] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [IsCredit]    BIT              NOT NULL,
    [Settings]    NVARCHAR (MAX)   NOT NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_BillingTransactionType_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_BillingTransactionType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_BillingTransactionType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);







