CREATE TABLE [VR_AccountBalance].[AccountUsageOverride] (
    [ID]                        BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountTypeID]             UNIQUEIDENTIFIER NOT NULL,
    [TransactionTypeID]         UNIQUEIDENTIFIER NOT NULL,
    [AccountID]                 VARCHAR (50)     NOT NULL,
    [PeriodStart]               DATETIME         NOT NULL,
    [PeriodEnd]                 DATETIME         NOT NULL,
    [OverriddenByTransactionID] BIGINT           NOT NULL,
    [CreatedTime]               DATETIME         CONSTRAINT [DF_AccountUsageOverride_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                 ROWVERSION       NULL,
    CONSTRAINT [PK_AccountUsageOverride] PRIMARY KEY CLUSTERED ([ID] ASC)
);

